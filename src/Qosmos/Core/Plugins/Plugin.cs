// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Logging;

namespace Qosmos.Core.Plugins;

/// <summary>
/// Represents an abstract base class for plugins, providing lifecycle methods for setup, start, and stop operations.
/// </summary>
public abstract class Plugin : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets or sets a value indicating whether the plugin has been disposed.
    /// </summary>
    private bool IsDisposed { get; set; }

    /// <summary>
    /// Gets the logger instance for the plugin.
    /// </summary>
    public ILogger Logger { get; internal set; } = null!;

    /// <summary>
    /// Gets the unique identifier of the plugin.
    /// </summary>
    public PluginIdentifier Identifier { get; internal set; } = null!;

    /// <summary>
    /// Gets the manifest containing metadata and configuration details for the plugin.
    /// </summary>
    public PluginManifest Manifest { get; internal set; } = null!;

    /// <summary>
    /// Gets the current state of the plugin.
    /// </summary>
    public PluginState State { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the plugin is disabled.
    /// </summary>
    public bool IsDisabled =>
        State < PluginState.Setup;

    /// <summary>
    /// Gets a value indicating whether the plugin is enabled.
    /// </summary>
    public bool IsEnabled =>
        State >= PluginState.Setup;

    /// <summary>
    /// Performs setup operations for the plugin. Can be overridden by derived classes.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual ValueTask SetupAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Starts the plugin. Can be overridden by derived classes.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual ValueTask StartAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Stops the plugin. Can be overridden by derived classes.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual ValueTask StopAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Releases unmanaged resources and performs cleanup operations.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose.</param>
    protected virtual void Dispose(bool disposing)
    {
    }

    /// <summary>
    /// Releases unmanaged resources asynchronously and performs cleanup operations.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual ValueTask DisposeAsyncCore()
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Releases resources used by the plugin.
    /// </summary>
    public void Dispose()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;

        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases resources used by the plugin asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;

        await DisposeAsyncCore();
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs internal setup operations for the plugin.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the plugin is in an invalid state for setup.</exception>
    internal async ValueTask SetupInternalAsync(CancellationToken cancellationToken)
    {
        if (State is not PluginState.None && State is not PluginState.Disabled)
            throw new InvalidOperationException($"Plugin {Identifier} cannot be setup in state {State}. Expected: None or Disabled.");

        State = PluginState.Setup;

        try
        {
            await SetupAsync(cancellationToken);
        }
        catch (Exception e)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError(e, "An exception occurred while setting up plugin {Identifier}", Identifier);

            State = PluginState.Disabled;
        }
    }

    /// <summary>
    /// Performs internal start operations for the plugin.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the plugin is in an invalid state for starting.</exception>
    internal async ValueTask StartInternalAsync(CancellationToken cancellationToken)
    {
        if (State is not PluginState.Setup)
            throw new InvalidOperationException($"Plugin {Identifier} cannot be started in state {State}. Expected: Setup.");

        State = PluginState.Start;

        try
        {
            await StartAsync(cancellationToken);

            State = PluginState.Enabled;
        }
        catch (Exception e)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError(e, "An exception occurred while starting plugin {Identifier}", Identifier);

            State = PluginState.Disabled;
        }
    }

    /// <summary>
    /// Performs internal stop operations for the plugin.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal async ValueTask StopInternalAsync(CancellationToken cancellationToken)
    {
        State = PluginState.Shutdown;

        try
        {
            await StopAsync(cancellationToken);
        }
        catch (Exception e)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError(e, "An exception occurred while shutting down plugin {Identifier}", Identifier);
        }

        State = PluginState.Disabled;
    }
}
