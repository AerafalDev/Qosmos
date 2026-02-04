// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qosmos.Core.Plugins;

/// <summary>
/// Represents a plugin that is waiting to be loaded.
/// Contains information about the plugin's identifier, manifest, path, and dependencies.
/// </summary>
public sealed class PluginWaitingToLoad
{
    /// <summary>
    /// Gets the unique identifier of the plugin.
    /// </summary>
    public PluginIdentifier Identifier { get; }

    /// <summary>
    /// Gets the manifest containing metadata and configuration details for the plugin.
    /// </summary>
    public PluginManifest Manifest { get; }

    /// <summary>
    /// Gets the file path of the plugin.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets a value indicating whether the plugin is in the server class path.
    /// </summary>
    public bool IsInServerClassPath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginWaitingToLoad"/> class.
    /// </summary>
    /// <param name="path">The file path of the plugin.</param>
    /// <param name="manifest">The manifest containing metadata and configuration details for the plugin.</param>
    /// <param name="isInServerClassPath">Indicates whether the plugin is in the server class path.</param>
    public PluginWaitingToLoad(string path, PluginManifest manifest, bool isInServerClassPath)
    {
        Identifier = new PluginIdentifier(manifest);
        Path = path;
        Manifest = manifest;
        IsInServerClassPath = isInServerClassPath;
    }

    /// <summary>
    /// Creates a collection of sub-plugins waiting to be loaded based on the manifest's sub-plugins.
    /// </summary>
    /// <returns>A collection of <see cref="PluginWaitingToLoad"/> instances representing the sub-plugins.</returns>
    public IEnumerable<PluginWaitingToLoad> CreateSubPluginsWaitingToLoad()
    {
        var plugins = new List<PluginWaitingToLoad>(Manifest.SubPlugins.Count);

        foreach (var subPluginManifest in Manifest.SubPlugins)
        {
            subPluginManifest.Inherit(Manifest);
            plugins.Add(new PluginWaitingToLoad(Path, subPluginManifest, IsInServerClassPath));
        }

        return plugins;
    }

    /// <summary>
    /// Determines whether the plugin depends on another plugin with the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier of the plugin to check.</param>
    /// <returns><c>true</c> if the plugin depends on the specified identifier; otherwise, <c>false</c>.</returns>
    public bool DependsOn(PluginIdentifier identifier)
    {
        return Manifest.Dependencies.ContainsKey(identifier) || Manifest.OptionalDependencies.ContainsKey(identifier);
    }
}
