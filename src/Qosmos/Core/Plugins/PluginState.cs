// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qosmos.Core.Plugins;

/// <summary>
/// Represents the various states a plugin can be in during its lifecycle.
/// </summary>
public enum PluginState
{
    /// <summary>
    /// The plugin is in an undefined or uninitialized state.
    /// </summary>
    None,

    /// <summary>
    /// The plugin is shutting down.
    /// </summary>
    Shutdown,

    /// <summary>
    /// The plugin is disabled and not operational.
    /// </summary>
    Disabled,

    /// <summary>
    /// The plugin is being set up.
    /// </summary>
    Setup,

    /// <summary>
    /// The plugin is starting.
    /// </summary>
    Start,

    /// <summary>
    /// The plugin is enabled and fully operational.
    /// </summary>
    Enabled
}
