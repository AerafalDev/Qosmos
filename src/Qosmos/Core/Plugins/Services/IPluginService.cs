// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Semver;

namespace Qosmos.Core.Plugins.Services;

/// <summary>
/// Defines the contract for a service that manages plugins within the application.
/// </summary>
public interface IPluginService
{
    /// <summary>
    /// Gets the current state of the plugin service.
    /// </summary>
    PluginState State { get; }

    /// <summary>
    /// Sets up the plugin service asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous setup operation.</returns>
    ValueTask SetupAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Starts the plugin service asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    ValueTask StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops the plugin service asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    ValueTask StopAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all loaded plugins.
    /// </summary>
    /// <returns>An enumerable collection of loaded plugins.</returns>
    IEnumerable<Plugin> GetPlugins();

    /// <summary>
    /// Retrieves a specific plugin by its identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to retrieve.</param>
    /// <returns>The plugin if found; otherwise, null.</returns>
    Plugin? GetPlugin(PluginIdentifier identifier);

    /// <summary>
    /// Attempts to retrieve a specific plugin by its identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to retrieve.</param>
    /// <param name="plugin">When this method returns, contains the plugin if found; otherwise, null.</param>
    /// <returns>True if the plugin was found; otherwise, false.</returns>
    bool TryGetPlugin(PluginIdentifier identifier, [NotNullWhen(true)] out Plugin? plugin);

    /// <summary>
    /// Determines whether a plugin with the specified identifier and version range exists.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin.</param>
    /// <param name="range">The version range to check.</param>
    /// <returns>True if the plugin exists; otherwise, false.</returns>
    bool HasPlugin(PluginIdentifier identifier, SemVersionRange range);

    /// <summary>
    /// Reloads a plugin asynchronously.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to reload.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous reload operation. The task result indicates whether the reload was successful.</returns>
    ValueTask<bool> Reload(PluginIdentifier identifier, CancellationToken cancellationToken);

    /// <summary>
    /// Loads a plugin asynchronously.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to load.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous load operation. The task result indicates whether the load was successful.</returns>
    ValueTask<bool> LoadAsync(PluginIdentifier identifier, CancellationToken cancellationToken);

    /// <summary>
    /// Unloads a plugin asynchronously.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to unload.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous unload operation. The task result indicates whether the unload was successful.</returns>
    ValueTask<bool> UnloadAsync(PluginIdentifier identifier, CancellationToken cancellationToken);
}
