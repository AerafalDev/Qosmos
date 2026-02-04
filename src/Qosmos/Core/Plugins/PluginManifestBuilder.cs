// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Semver;

namespace Qosmos.Core.Plugins;

/// <summary>
/// A builder class for constructing instances of <see cref="PluginManifest"/>.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class PluginManifestBuilder
{
    public static readonly SemVersion CurrentVersion = typeof(PluginManifestBuilder).Assembly.GetName().Version is { } version
        ? SemVersion.FromVersion(version)
        : SemVersion.Parse("0.0.0-dev");

    private readonly string _group;
    private readonly string _name;
    private readonly string _main;
    private readonly Dictionary<PluginIdentifier, SemVersionRange> _dependencies;
    private readonly Dictionary<PluginIdentifier, SemVersionRange> _optionalDependencies;
    private readonly Dictionary<PluginIdentifier, SemVersionRange> _loadBefore;

    private string? _description;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginManifestBuilder"/> class with the specified group, name, and main entry point.
    /// </summary>
    /// <param name="group">The group of the plugin.</param>
    /// <param name="name">The name of the plugin.</param>
    /// <param name="main">The main entry point of the plugin.</param>
    public PluginManifestBuilder(string group, string name, string main)
    {
        _group = group;
        _name = name;
        _main = main;
        _dependencies = [];
        _optionalDependencies = [];
        _loadBefore = [];
    }

    /// <summary>
    /// Sets the description of the plugin.
    /// </summary>
    /// <param name="description">The description of the plugin.</param>
    /// <returns>The current <see cref="PluginManifestBuilder"/> instance.</returns>
    public PluginManifestBuilder Description(string description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Adds a dependency on another plugin of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the dependent plugin.</typeparam>
    /// <returns>The current <see cref="PluginManifestBuilder"/> instance.</returns>
    public PluginManifestBuilder DependsOn<T>()
        where T : Plugin
    {
        _dependencies.Add(new PluginIdentifier("Hytale", typeof(T).Name), SemVersionRange.Empty);
        return this;
    }

    /// <summary>
    /// Adds an optional dependency on another plugin of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the optional dependent plugin.</typeparam>
    /// <returns>The current <see cref="PluginManifestBuilder"/> instance.</returns>
    public PluginManifestBuilder OptionalDependsOn<T>()
        where T : Plugin
    {
        _optionalDependencies.Add(new PluginIdentifier("Hytale", typeof(T).Name), SemVersionRange.Empty);
        return this;
    }

    /// <summary>
    /// Specifies that another plugin of type <typeparamref name="T"/> should be loaded before this plugin.
    /// </summary>
    /// <typeparam name="T">The type of the plugin to load before.</typeparam>
    /// <returns>The current <see cref="PluginManifestBuilder"/> instance.</returns>
    public PluginManifestBuilder LoadBefore<T>()
        where T : Plugin
    {
        _loadBefore.Add(new PluginIdentifier("Hytale", typeof(T).Name), SemVersionRange.Empty);
        return this;
    }

    /// <summary>
    /// Builds and returns a new <see cref="PluginManifest"/> instance based on the current state of the builder.
    /// </summary>
    /// <returns>A new <see cref="PluginManifest"/> instance.</returns>
    public PluginManifest Build()
    {
        return new PluginManifest(
            _group,
            _name,
            CurrentVersion,
            _description,
            [],
            null,
            _main,
            null,
            _dependencies,
            _optionalDependencies,
            _loadBefore,
            [],
            false,
            false);
    }

    /// <summary>
    /// Returns a string representation of the plugin manifest builder.
    /// </summary>
    /// <returns>A string in the format "PluginManifestBuilder(...)" with the builder's properties.</returns>
    public override string ToString()
    {
        return
            $"PluginManifestBuilder(" +
            $"Group={_group}, " +
            $"Name={_name}, " +
            $"Version={CurrentVersion}, " +
            $"Description={_description}, " +
            $"Main={_main}, " +
            $"Dependencies={_dependencies.Count}, " +
            $"OptionalDependencies={_optionalDependencies.Count}, " +
            $"LoadBefore={_loadBefore.Count}, " +
            $")";
    }
}
