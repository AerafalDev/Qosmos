// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Semver;

namespace Qosmos.Core.Plugins.Services;

/// <summary>
/// Provides services for managing plugins, including loading, unloading, and lifecycle management.
/// </summary>
public sealed class PluginService : IPluginService
{
    private static readonly Assembly s_coreAssembly = typeof(PluginService).Assembly;

    private readonly ILogger<PluginService> _logger;
    private readonly IServiceProvider _provider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Lock _stateLock;
    private readonly ConcurrentBag<PluginWaitingToLoad> _corePlugins;
    private readonly ConcurrentDictionary<PluginIdentifier, PluginManifest> _availablePlugins;
    private readonly ConcurrentDictionary<PluginIdentifier, Plugin> _plugins;

    private List<PluginWaitingToLoad> _loadOrder;
    private ConcurrentDictionary<PluginIdentifier, Plugin> _loadingPlugins;

    /// <summary>
    /// Gets or sets the current state of the plugin service.
    /// </summary>
    public PluginState State
    {
        get { lock (_stateLock) return field; }
        private set { lock (_stateLock) field = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging plugin service operations.</param>
    /// <param name="provider">The service provider for dependency injection.</param>
    /// <param name="loggerFactory">The logger factory for creating loggers.</param>
    /// <param name="corePlugins">The collection of core plugins to be loaded.</param>
    public PluginService(ILogger<PluginService> logger, IServiceProvider provider, ILoggerFactory loggerFactory, IEnumerable<PluginWaitingToLoad> corePlugins)
    {
        _logger = logger;
        _provider = provider;
        _loggerFactory = loggerFactory;
        _corePlugins = new ConcurrentBag<PluginWaitingToLoad>(corePlugins);
        _availablePlugins = [];
        _plugins = [];
        _loadOrder = [];
        _loadingPlugins = [];
        _stateLock = new Lock();
    }

    /// <summary>
    /// Sets up the plugin service by loading and validating plugins.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous setup operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the service is not in the expected state.</exception>
    public async ValueTask SetupAsync(CancellationToken cancellationToken)
    {
        if (State is not PluginState.None)
            throw new InvalidOperationException($"PluginService cannot be setup in state {State}. Expected state: None.");

        State = PluginState.Setup;

        var pendingPlugins = new Dictionary<PluginIdentifier, PluginWaitingToLoad>();

        _availablePlugins.Clear();

        _logger.LogInformation("Loading pending core plugins");

        foreach (var corePlugin in _corePlugins)
        {
            if (CanLoadOnBoot(corePlugin.Manifest))
                LoadPendingPlugin(pendingPlugins, corePlugin);
            else
                _availablePlugins.TryAdd(corePlugin.Identifier, corePlugin.Manifest);
        }

        // LoadPluginsFromDirectory(pendingPlugins, PluginsPath, true, _availablePlugins);

        foreach (var identifier in _plugins.Keys)
        {
            pendingPlugins.Remove(identifier);

            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Skipping loading of {Identifier} because it is already loaded", identifier);
        }

        var failedPluginIdentifiers = new List<PluginIdentifier>();

        foreach (var (identifier, pendingPlugin) in pendingPlugins)
        {
            try
            {
                ValidatePluginDependencies(pendingPlugin, pendingPlugins);
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError(e, "Failed to validate dependencies for plugin {Identifier}. Plugin will be skipped", identifier);

                failedPluginIdentifiers.Add(identifier);
            }
        }

        foreach (var identifier in failedPluginIdentifiers)
            pendingPlugins.Remove(identifier);

        _loadOrder = CalculateLoadOrder(pendingPlugins);
        _loadingPlugins = [];

        foreach (var (identifier, pendingPlugin) in pendingPlugins)
            _availablePlugins.TryAdd(identifier, pendingPlugin.Manifest);

        foreach (var pendingPlugin in _loadOrder)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("- {Identifier}", pendingPlugin.Identifier);

            var plugin = CreatePluginInstance(pendingPlugin);

            _plugins.TryAdd(plugin.Identifier, plugin);
            _loadingPlugins.TryAdd(plugin.Identifier, plugin);
        }

        foreach (var pendingPlugin in _loadOrder)
        {
            if (_loadingPlugins.TryGetValue(pendingPlugin.Identifier, out var plugin) && !await SetupPluginAsync(plugin, cancellationToken))
                _loadingPlugins.Remove(pendingPlugin.Identifier, out _);
        }
    }

    /// <summary>
    /// Starts all loaded plugins.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous start operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the service is not in the expected state.</exception>
    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        if (State is not PluginState.Setup)
            throw new InvalidOperationException($"PluginService cannot be started in state {State}. Expected state: Setup.");

        State = PluginState.Start;

        foreach (var pendingPlugin in _loadOrder)
        {
            if (_loadingPlugins.TryGetValue(pendingPlugin.Identifier, out var plugin) && !await StartPluginAsync(plugin, cancellationToken))
                _loadingPlugins.Remove(pendingPlugin.Identifier, out _);
        }

        _loadOrder = [];
        _loadingPlugins = [];
    }

    /// <summary>
    /// Stops all loaded plugins and clears the plugin collection.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    public async ValueTask StopAsync(CancellationToken cancellationToken)
    {
        State = PluginState.Shutdown;

        foreach (var plugin in _plugins.Values)
        {
            await using (plugin)
            {
                if (plugin.State is PluginState.Enabled)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("Shutting down plugin {Identifier}", plugin.Identifier);

                    await plugin.StopInternalAsync(cancellationToken);

                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("Shut down plugin {Identifier}", plugin.Identifier);
                }
            }
        }

        _plugins.Clear();
    }

    /// <summary>
    /// Retrieves all currently loaded plugins.
    /// </summary>
    /// <returns>An enumerable of loaded plugins.</returns>
    public IEnumerable<Plugin> GetPlugins()
    {
        return _plugins.Values;
    }

    /// <summary>
    /// Retrieves a plugin by its identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to retrieve.</param>
    /// <returns>The plugin instance, or <c>null</c> if not found.</returns>
    public Plugin? GetPlugin(PluginIdentifier identifier)
    {
        return _plugins.GetValueOrDefault(identifier);
    }

    /// <summary>
    /// Attempts to retrieve a plugin by its identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to retrieve.</param>
    /// <param name="plugin">The retrieved plugin, if found.</param>
    /// <returns><c>true</c> if the plugin was found; otherwise, <c>false</c>.</returns>
    public bool TryGetPlugin(PluginIdentifier identifier, [NotNullWhen(true)] out Plugin? plugin)
    {
        return _plugins.TryGetValue(identifier, out plugin);
    }

    /// <summary>
    /// Checks if a plugin with the specified identifier and version range is loaded.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin.</param>
    /// <param name="range">The version range to check.</param>
    /// <returns><c>true</c> if the plugin is loaded and satisfies the version range; otherwise, <c>false</c>.</returns>
    public bool HasPlugin(PluginIdentifier identifier, SemVersionRange range)
    {
        return _plugins.TryGetValue(identifier, out var plugin) &&
               plugin.Manifest.Version is { } version && version.Satisfies(range);
    }

    /// <summary>
    /// Reloads a plugin by unloading and then loading it again.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to reload.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><c>true</c> if the plugin was successfully reloaded; otherwise, <c>false</c>.</returns>
    public async ValueTask<bool> Reload(PluginIdentifier identifier, CancellationToken cancellationToken)
    {
        return await UnloadAsync(identifier, cancellationToken) && await LoadAsync(identifier, cancellationToken);
    }

    /// <summary>
    /// Loads a plugin by its identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to load.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><c>true</c> if the plugin was successfully loaded; otherwise, <c>false</c>.</returns>
    public ValueTask<bool> LoadAsync(PluginIdentifier identifier, CancellationToken cancellationToken)
    {
        return FindAndLoadPluginAsync(identifier, cancellationToken);
    }

    /// <summary>
    /// Unloads a plugin by its identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to unload.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><c>true</c> if the plugin was successfully unloaded; otherwise, <c>false</c>.</returns>
    public async ValueTask<bool> UnloadAsync(PluginIdentifier identifier, CancellationToken cancellationToken)
    {
        if (!_plugins.Remove(identifier, out var plugin))
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Cannot unload plugin {Identifier}: plugin not found", identifier);

            return false;
        }

        if (plugin.State is not PluginState.Enabled)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Cannot unload plugin {Identifier}: plugin is in state {State}", identifier, plugin.State);

            return false;
        }

        await plugin.StopInternalAsync(cancellationToken);

        _availablePlugins.Remove(identifier, out _);

        return true;
    }

    /// <summary>
    /// Attempts to find and load a plugin asynchronously by its identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to find and load.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the plugin was successfully loaded; otherwise, <c>false</c>.</returns>
    private ValueTask<bool> FindAndLoadPluginAsync(PluginIdentifier identifier, CancellationToken cancellationToken)
    {
        return _corePlugins.FirstOrDefault(x => x.Identifier.Equals(identifier)) is { } corePlugin
            ? LoadPluginAsync(corePlugin, cancellationToken)
            : ValueTask.FromResult(false);

        // FindPluginInDirectoryAsync(identifier, PluginsPath, cancellationToken);
    }

    /// <summary>
    /// Loads a plugin asynchronously from the specified pending plugin.
    /// </summary>
    /// <param name="pendingPlugin">The pending plugin to load.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the plugin was successfully loaded; otherwise, <c>false</c>.</returns>
    private async ValueTask<bool> LoadPluginAsync(PluginWaitingToLoad pendingPlugin, CancellationToken cancellationToken)
    {
        var plugin = CreatePluginInstance(pendingPlugin);

        return _plugins.TryAdd(plugin.Identifier, plugin) &&
               await SetupPluginAsync(plugin, cancellationToken) &&
               await StartPluginAsync(plugin, cancellationToken);
    }

    /// <summary>
    /// Creates a plugin instance from the specified pending plugin.
    /// </summary>
    /// <param name="pendingPlugin">The pending plugin to create an instance from.</param>
    /// <returns>The created plugin instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the plugin manifest is invalid or the plugin type is not found.</exception>
    private Plugin CreatePluginInstance(PluginWaitingToLoad pendingPlugin)
    {
        if (!pendingPlugin.IsInServerClassPath)
            throw new NotSupportedException("Only core plugins are enabled at this time.");

        if (string.IsNullOrEmpty(pendingPlugin.Manifest.Main))
            throw new InvalidOperationException($"Plugin {pendingPlugin.Identifier} has no main entry point defined in manifest.");

        var pluginType = s_coreAssembly.GetType(pendingPlugin.Manifest.Main);

        if (pluginType is null)
            throw new InvalidOperationException($"Plugin {pendingPlugin.Identifier} cannot find type '{pendingPlugin.Manifest.Main}' in assembly.");

        var pluginInstance = (Plugin)ActivatorUtilities.CreateInstance(_provider, pluginType);

        pluginInstance.Manifest = pendingPlugin.Manifest;
        pluginInstance.Identifier = pendingPlugin.Identifier;
        pluginInstance.Logger = _loggerFactory.CreateLogger(string.Concat(pendingPlugin.Manifest.Name, "|P"));

        return pluginInstance;
    }

    /// <summary>
    /// Sets up the specified plugin asynchronously.
    /// </summary>
    /// <param name="plugin">The plugin to set up.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the setup was successful; otherwise, <c>false</c>.</returns>
    private async ValueTask<bool> SetupPluginAsync(Plugin plugin, CancellationToken cancellationToken)
    {
        if (plugin.State is PluginState.None && DependenciesMatchState(plugin, PluginState.Setup, PluginState.Setup))
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Setting up plugin {Identifier}", plugin.Identifier);

            await plugin.SetupInternalAsync(cancellationToken);

            if (plugin.State is not PluginState.Disabled)
                return true;
        }

        await plugin.StopInternalAsync(cancellationToken);

        _plugins.Remove(plugin.Identifier, out _);

        return false;
    }

    /// <summary>
    /// Starts the specified plugin asynchronously.
    /// </summary>
    /// <param name="plugin">The plugin to start.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the start was successful; otherwise, <c>false</c>.</returns>
    private async ValueTask<bool> StartPluginAsync(Plugin plugin, CancellationToken cancellationToken)
    {
        if (plugin.State is PluginState.Setup && DependenciesMatchState(plugin, PluginState.Enabled, PluginState.Start))
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Starting plugin {Identifier}", plugin.Identifier);

            await plugin.StartInternalAsync(cancellationToken);

            if (plugin.State is not PluginState.Disabled)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("Enabled plugin {Identifier}", plugin.Identifier);

                return true;
            }
        }

        await plugin.StopInternalAsync(cancellationToken);

        _plugins.Remove(plugin.Identifier, out _);

        return false;
    }

    /// <summary>
    /// Validates whether the dependencies of the specified plugin match the required state.
    /// </summary>
    /// <param name="plugin">The plugin to validate.</param>
    /// <param name="requiredState">The required state of the dependencies.</param>
    /// <param name="currentStage">The current stage of the plugin lifecycle.</param>
    /// <returns><c>true</c> if the dependencies match the required state; otherwise, <c>false</c>.</returns>
    private bool DependenciesMatchState(Plugin plugin, PluginState requiredState, PluginState currentStage)
    {
        foreach (var dependencyId in plugin.Manifest.Dependencies.Keys)
        {
            if (!_plugins.TryGetValue(dependencyId, out var dependencyPlugin))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Plugin {PluginName} is missing required dependency {DependencyName} at stage {Stage}", plugin.Identifier.Name, dependencyId.Name, currentStage);
                    _logger.LogError("Plugin {PluginName} will be DISABLED", plugin.Identifier.Name);
                }

                return false;
            }

            if (dependencyPlugin.State != requiredState)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Plugin {PluginName} dependency {DependencyName} is in invalid state {DependencyState} (expected {RequiredState}) at stage {Stage}", plugin.Identifier.Name, dependencyId.Name, dependencyPlugin.State, requiredState, currentStage);
                    _logger.LogError("Plugin {PluginName} will be DISABLED", plugin.Identifier.Name);
                }

                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Validates the dependencies of a plugin to ensure they meet the required conditions.
    /// </summary>
    /// <param name="plugin">The plugin whose dependencies are being validated.</param>
    /// <param name="pendingPlugins">A dictionary of plugins that are pending to be loaded, keyed by their identifiers.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the server version does not satisfy the required range,
    /// or if a required dependency is missing, has no version information,
    /// or does not satisfy the required version range.
    /// </exception>
    private void ValidatePluginDependencies(PluginWaitingToLoad plugin, Dictionary<PluginIdentifier, PluginWaitingToLoad>? pendingPlugins)
    {
        var serverVersion = PluginManifestBuilder.CurrentVersion;
        var requiredServerVersionRange = plugin.Manifest.ServerVersion;

        if (requiredServerVersionRange is not null && serverVersion is not null && !serverVersion.Satisfies(requiredServerVersionRange))
            throw new InvalidOperationException($"Failed to load plugin {plugin.Identifier}: server version {serverVersion} does not satisfy required version range {requiredServerVersionRange}.");

        foreach (var (dependencyId, requiredVersionRange) in plugin.Manifest.Dependencies)
        {
            PluginManifest? dependencyManifest = null;

            if (pendingPlugins is not null && pendingPlugins.TryGetValue(dependencyId, out var pendingDependency))
                dependencyManifest = pendingDependency.Manifest;

            if (dependencyManifest is null && _plugins.TryGetValue(dependencyId, out var loadedDependency))
                dependencyManifest = loadedDependency.Manifest;

            if (dependencyManifest is null)
                throw new InvalidOperationException($"Failed to load plugin {plugin.Identifier}: required dependency '{dependencyId}' not found.");

            if (dependencyManifest.Version is null)
                throw new InvalidOperationException($"Failed to load plugin {plugin.Identifier}: dependency '{dependencyId}' has no version information.");

            if (!dependencyManifest.Version.Satisfies(requiredVersionRange))
                throw new InvalidOperationException($"Failed to load plugin {plugin.Identifier}: dependency '{dependencyId}' version {dependencyManifest.Version} does not satisfy required range {requiredVersionRange}.");
        }
    }

    /// <summary>
    /// Determines whether the specified plugin manifest allows the plugin to be loaded on boot.
    /// </summary>
    /// <param name="manifest">The plugin manifest to evaluate.</param>
    /// <returns><c>true</c> if the plugin can be loaded on boot; otherwise, <c>false</c>.</returns>
    private static bool CanLoadOnBoot(PluginManifest manifest)
    {
        return !manifest.DisabledByDefault;
    }

    /// <summary>
    /// Adds a plugin to the pending plugins collection, including its sub-plugins.
    /// </summary>
    /// <param name="pendingPlugins">The collection of pending plugins to populate.</param>
    /// <param name="plugin">The plugin to add to the pending collection.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a plugin with the same identifier is already pending to be loaded.
    /// </exception>
    private static void LoadPendingPlugin(Dictionary<PluginIdentifier, PluginWaitingToLoad> pendingPlugins, PluginWaitingToLoad plugin)
    {
        if (!pendingPlugins.TryAdd(plugin.Identifier, plugin))
            throw new InvalidOperationException($"Duplicate plugin identifier; {plugin.Identifier}. A plugin with this identifier is already pending to be loaded.");

        foreach (var subPlugin in plugin.CreateSubPluginsWaitingToLoad())
            LoadPendingPlugin(pendingPlugins, subPlugin);
    }

    /// <summary>
    /// Calculates the load order of plugins based on their dependencies.
    /// </summary>
    /// <param name="pendingPlugins">A dictionary of plugins waiting to be loaded, keyed by their identifiers.</param>
    /// <returns>An ordered collection of <see cref="PluginWaitingToLoad"/> instances representing the load order.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there are cyclic dependencies or missing dependencies that cannot be resolved.
    /// </exception>
    public static List<PluginWaitingToLoad> CalculateLoadOrder(Dictionary<PluginIdentifier, PluginWaitingToLoad> pendingPlugins)
    {
        var nodes = pendingPlugins
            .ToDictionary(static kvp => kvp.Key, static kvp => new Node([], kvp.Value));

        var classpathPlugins = pendingPlugins
            .Where(static kvp => kvp.Value.IsInServerClassPath)
            .Select(static kvp => kvp.Key)
            .ToHashSet();

        var missingDependencies = new Dictionary<PluginIdentifier, List<PluginIdentifier>>();

        foreach (var node in nodes.Values)
        {
            foreach (var dependencyId in node.Plugin.Manifest.Dependencies.Keys)
            {
                if (nodes.ContainsKey(dependencyId))
                {
                    node.Edge.Add(dependencyId);
                    continue;
                }

                if (!missingDependencies.TryGetValue(node.Plugin.Identifier, out var dependencies))
                    missingDependencies.Add(node.Plugin.Identifier, dependencies = []);

                dependencies.Add(dependencyId);
            }

            foreach (var identifier in node.Plugin.Manifest.OptionalDependencies.Keys.Where(identifier => nodes.ContainsKey(identifier)))
                node.Edge.Add(identifier);

            if (!node.Plugin.IsInServerClassPath)
                node.Edge.AddRange(classpathPlugins);
        }

        var missingLoadBefore = new Dictionary<PluginIdentifier, List<PluginIdentifier>>();

        foreach (var (key, value) in pendingPlugins)
        {
            var manifest = value.Manifest;

            foreach (var targetId in manifest.LoadBefore.Keys)
            {
                if (nodes.TryGetValue(targetId, out var node))
                {
                    node.Edge.Add(key);
                    continue;
                }

                if (!missingLoadBefore.TryGetValue(key, out var loadBefore))
                    missingLoadBefore.Add(key, loadBefore = []);

                loadBefore.Add(targetId);
            }
        }

        StringBuilder builder;

        if (missingDependencies.Count is 0 && missingLoadBefore.Count is 0)
        {
            var loadOrder = new List<PluginWaitingToLoad>(nodes.Count);

            while (nodes.Count > 0)
            {
                var didWork = false;
                var nodesToRemove = new List<PluginIdentifier>();

                foreach (var (key, value) in nodes)
                {
                    if (value.Edge.Count is 0)
                    {
                        didWork = true;
                        nodesToRemove.Add(key);
                        loadOrder.Add(value.Plugin);
                    }
                }

                foreach (var key in nodesToRemove)
                {
                    nodes.Remove(key);

                    foreach (var node in nodes.Values)
                        node.Edge.Remove(key);
                }

                if (!didWork)
                {
                    builder = new StringBuilder()
                        .AppendLine("Found cyclic dependency between plugins:")
                        .AppendLine("This usually means plugins have circular dependencies that cannot be resolved.")
                        .AppendLine();

                    foreach (var (key, value) in nodes)
                        builder
                            .Append('\t')
                            .Append(key)
                            .Append(" waiting on: ")
                            .AppendLine(string.Join(", ", value.Edge));

                    throw new InvalidOperationException(builder.ToString());
                }
            }

            return loadOrder;
        }

        builder = new StringBuilder();

        if (missingDependencies.Count > 0)
        {
            builder.AppendLine("Missing required dependencies:");

            foreach (var (key, value) in missingDependencies)
                builder
                    .Append('\t')
                    .Append(key)
                    .Append(" requires: ")
                    .AppendLine(string.Join(", ", value));
        }

        if (missingLoadBefore.Count > 0)
        {
            builder.AppendLine("Missing load before targets:");

            foreach (var (key, value) in missingLoadBefore)
                builder
                    .Append('\t')
                    .Append(key)
                    .Append(" load before: ")
                    .AppendLine(string.Join(", ", value));
        }

        throw new InvalidOperationException(builder.ToString());
    }

    /// <summary>
    /// Represents a node in the dependency graph for calculating plugin load order.
    /// </summary>
    /// <param name="Edge">The list of dependencies for the plugin.</param>
    /// <param name="Plugin">The plugin associated with the node.</param>
    private sealed record Node(List<PluginIdentifier> Edge, PluginWaitingToLoad Plugin);
}
