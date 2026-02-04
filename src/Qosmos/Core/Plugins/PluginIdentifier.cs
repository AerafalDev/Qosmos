// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Qosmos.Core.Plugins;

/// <summary>
/// Represents a unique identifier for a plugin, consisting of a group and a name.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class PluginIdentifier : IEquatable<PluginIdentifier>, IParsable<PluginIdentifier>
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
    /// Initializes a new instance of the <see cref="PluginIdentifier"/> class with the specified group and name.
    /// </summary>
    /// <param name="group">The group of the plugin.</param>
    /// <param name="name">The name of the plugin.</param>
    public PluginIdentifier(string group, string name)
    {
        Group = group;
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginIdentifier"/> class using a <see cref="PluginManifest"/>.
    /// </summary>
    /// <param name="manifest">The plugin manifest containing the group and name.</param>
    public PluginIdentifier(PluginManifest manifest)
    {
        Group = manifest.Group;
        Name = manifest.Name;
    }

    /// <summary>
    /// Determines whether the specified <see cref="PluginIdentifier"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The other <see cref="PluginIdentifier"/> to compare with.</param>
    /// <returns><c>true</c> if the specified <see cref="PluginIdentifier"/> is equal to the current instance; otherwise, <c>false</c>.</returns>
    public bool Equals(PluginIdentifier? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Group == other.Group && Name == other.Name;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is PluginIdentifier other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for the current instance.
    /// </summary>
    /// <returns>The hash code for the current instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Group, Name);
    }

    /// <summary>
    /// Returns a string representation of the plugin identifier in the format "Group:Name".
    /// </summary>
    /// <returns>A string representation of the plugin identifier.</returns>
    public override string ToString()
    {
        return $"{Group}:{Name}";
    }

    /// <summary>
    /// Parses a string into a <see cref="PluginIdentifier"/> object.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>A <see cref="PluginIdentifier"/> object.</returns>
    /// <exception cref="ArgumentException">Thrown if the string is not a valid plugin identifier.</exception>
    public static PluginIdentifier Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new ArgumentException($"Invalid plugin identifier '{s}'.", nameof(s));
    }

    /// <summary>
    /// Tries to parse a string into a <see cref="PluginIdentifier"/> object.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="PluginIdentifier"/> object, or <c>null</c> if parsing failed.</param>
    /// <returns><c>true</c> if the string was successfully parsed; otherwise, <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PluginIdentifier result)
    {
        result = null;

        if (string.IsNullOrEmpty(s))
            return false;

        var parts = s.Split(':');

        if (parts.Length is not 2)
            return false;

        result = new PluginIdentifier(parts[0], parts[1]);
        return true;
    }
}
