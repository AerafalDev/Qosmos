// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qosmos.Core.Plugins;

/// <summary>
/// Represents an abstract base class for plugins, providing lifecycle methods for setup, start, and stop operations.
/// </summary>
public abstract class Plugin
{
    /// <summary>
    /// Asynchronously sets up the plugin. This method is called before the plugin starts.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public virtual ValueTask SetupAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Asynchronously starts the plugin. This method is called after the setup is complete.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public virtual ValueTask StartAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Asynchronously stops the plugin. This method is called when the plugin is being shut down.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public virtual ValueTask StopAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
