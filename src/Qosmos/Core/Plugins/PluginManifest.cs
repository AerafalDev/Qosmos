// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Semver;

namespace Qosmos.Core.Plugins;

/// <summary>
/// Represents the manifest of a plugin, containing metadata and configuration details.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class PluginManifest
{
    /// <summary>
    /// Gets or sets the group of the plugin.
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// Gets or sets the name of the plugin.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the version of the plugin.
    /// </summary>
    public SemVersion? Version { get; set; }

    /// <summary>
    /// Gets or sets the description of the plugin.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the list of authors of the plugin.
    /// </summary>
    public List<PluginAuthor> Authors { get; set; }

    /// <summary>
    /// Gets or sets the website URL of the plugin.
    /// </summary>
    public Uri? Website { get; set; }

    /// <summary>
    /// Gets or sets the main entry point of the plugin.
    /// </summary>
    public string? Main { get; set; }

    /// <summary>
    /// Gets or sets the server version range required by the plugin.
    /// </summary>
    public SemVersionRange? ServerVersion { get; set; }

    /// <summary>
    /// Gets or sets the dependencies required by the plugin.
    /// </summary>
    public Dictionary<PluginIdentifier, SemVersionRange> Dependencies { get; set; }

    /// <summary>
    /// Gets or sets the optional dependencies of the plugin.
    /// </summary>
    public Dictionary<PluginIdentifier, SemVersionRange> OptionalDependencies { get; set; }

    /// <summary>
    /// Gets or sets the plugins that should be loaded before this plugin.
    /// </summary>
    public Dictionary<PluginIdentifier, SemVersionRange> LoadBefore { get; set; }

    /// <summary>
    /// Gets or sets the list of sub-plugins included in this plugin.
    /// </summary>
    public List<PluginManifest> SubPlugins { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the plugin is disabled by default.
    /// </summary>
    public bool DisabledByDefault { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the plugin includes an asset pack.
    /// </summary>
    public bool IncludesAssetPack { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginManifest"/> class with default values.
    /// </summary>
    public PluginManifest()
    {
        Group = string.Empty;
        Name = string.Empty;
        Authors = [];
        Dependencies = [];
        OptionalDependencies = [];
        LoadBefore = [];
        SubPlugins = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginManifest"/> class with the specified values.
    /// </summary>
    /// <param name="group">The group of the plugin.</param>
    /// <param name="name">The name of the plugin.</param>
    /// <param name="version">The version of the plugin.</param>
    /// <param name="description">The description of the plugin.</param>
    /// <param name="authors">The list of authors of the plugin.</param>
    /// <param name="website">The website URL of the plugin.</param>
    /// <param name="main">The main entry point of the plugin.</param>
    /// <param name="serverVersion">The server version range required by the plugin.</param>
    /// <param name="dependencies">The dependencies required by the plugin.</param>
    /// <param name="optionalDependencies">The optional dependencies of the plugin.</param>
    /// <param name="loadBefore">The plugins that should be loaded before this plugin.</param>
    /// <param name="subPlugins">The list of sub-plugins included in this plugin.</param>
    /// <param name="disabledByDefault">A value indicating whether the plugin is disabled by default.</param>
    /// <param name="includesAssetPack">A value indicating whether the plugin includes an asset pack.</param>
    public PluginManifest(
        string group,
        string name,
        SemVersion? version,
        string? description,
        List<PluginAuthor> authors,
        Uri? website,
        string? main,
        SemVersionRange? serverVersion,
        Dictionary<PluginIdentifier, SemVersionRange> dependencies,
        Dictionary<PluginIdentifier, SemVersionRange> optionalDependencies,
        Dictionary<PluginIdentifier, SemVersionRange> loadBefore,
        List<PluginManifest> subPlugins,
        bool disabledByDefault,
        bool includesAssetPack)
    {
        Group = group;
        Name = name;
        Version = version;
        Description = description;
        Authors = authors;
        Website = website;
        Main = main;
        ServerVersion = serverVersion;
        Dependencies = dependencies;
        OptionalDependencies = optionalDependencies;
        LoadBefore = loadBefore;
        SubPlugins = subPlugins;
        DisabledByDefault = disabledByDefault;
        IncludesAssetPack = includesAssetPack;
    }

    /// <summary>
    /// Inherits properties from another <see cref="PluginManifest"/> instance.
    /// </summary>
    /// <param name="manifest">The manifest to inherit properties from.</param>
    public void Inherit(PluginManifest manifest)
    {
        if (string.IsNullOrEmpty(Group))
            Group = manifest.Group;

        Version ??= manifest.Version;

        if (string.IsNullOrEmpty(Description))
            Description = manifest.Description;

        if (Authors.Count is 0)
            Authors = manifest.Authors;

        if (string.IsNullOrEmpty(Website?.ToString()))
            Website = manifest.Website;

        if (!DisabledByDefault)
            DisabledByDefault = manifest.DisabledByDefault;

        Dependencies.Add(new PluginIdentifier(manifest), manifest.Version?.ToString() is { } v ? SemVersionRange.Parse(v) : SemVersionRange.Empty);
    }

    /// <summary>
    /// Returns a string representation of the plugin manifest.
    /// </summary>
    /// <returns>A string in the format "PluginManifest(...)" with the manifest's properties.</returns>
    public override string ToString()
    {
        return
            $"PluginManifest(" +
            $"Group={Group}, " +
            $"Name={Name}, " +
            $"Version={Version}, " +
            $"Description={Description}, " +
            $"Authors={Authors.Count}, " +
            $"Website={Website}, " +
            $"Main={Main}, " +
            $"ServerVersion={ServerVersion}, " +
            $"Dependencies={Dependencies.Count}, " +
            $"OptionalDependencies={OptionalDependencies.Count}, " +
            $"LoadBefore={LoadBefore.Count}, " +
            $"SubPlugins={SubPlugins.Count}, " +
            $"DisabledByDefault={DisabledByDefault}, " +
            $"IncludesAssetPack={IncludesAssetPack}" +
            $")";
    }
}
