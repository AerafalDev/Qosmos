// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Qosmos.Core.Plugins;

/// <summary>
/// Represents the author of a plugin, including their name, email, and URL.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class PluginAuthor
{
    /// <summary>
    /// Gets or sets the name of the plugin author.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the email address of the plugin author.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the URL associated with the plugin author.
    /// </summary>
    public Uri? Url { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginAuthor"/> class.
    /// </summary>
    public PluginAuthor()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginAuthor"/> class with the specified name, email, and URL.
    /// </summary>
    /// <param name="name">The name of the plugin author.</param>
    /// <param name="email">The email address of the plugin author.</param>
    /// <param name="url">The URL associated with the plugin author.</param>
    public PluginAuthor(string? name, string? email, Uri? url)
    {
        Name = name;
        Email = email;
        Url = url;
    }

    /// <summary>
    /// Returns a string representation of the plugin author.
    /// </summary>
    /// <returns>A string representation of the plugin author in the format "PluginAuthor(Name=..., Email=..., Url=...)".</returns>
    public override string ToString()
    {
        return $"PluginAuthor(Name={Name}, Email={Email}, Url={Url})";
    }
}
