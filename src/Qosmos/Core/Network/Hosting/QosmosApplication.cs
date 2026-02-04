// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Qosmos.Core.Plugins.Services;

namespace Qosmos.Core.Network.Hosting;

/// <summary>
/// Represents the main application class for a Qosmos application.
/// </summary>
internal sealed class QosmosApplication : IDisposable, IAsyncDisposable
{
    private readonly IHost _host;

    /// <summary>
    /// The application's configured services.
    /// </summary>
    public IServiceProvider Services =>
        _host.Services;

    /// <summary>
    /// The application's configured <see cref="IConfiguration"/>.
    /// </summary>
    public IConfiguration Configuration =>
        _host.Services.GetRequiredService<IConfiguration>();

    /// <summary>
    /// The application's configured <see cref="IWebHostEnvironment"/>.
    /// </summary>
    public IWebHostEnvironment Environment =>
        _host.Services.GetRequiredService<IWebHostEnvironment>();

    /// <summary>
    /// Allows consumers to be notified of application lifetime events.
    /// </summary>
    public IHostApplicationLifetime Lifetime =>
        _host.Services.GetRequiredService<IHostApplicationLifetime>();

    /// <summary>
    /// Initializes a new instance of the <see cref="QosmosApplication"/> class.
    /// </summary>
    /// <param name="host">The host instance that manages the application's lifetime and services.</param>
    public QosmosApplication(IHost host)
    {
        _host = host;
    }

    /// <summary>
    /// Runs the application asynchronously, starting the host and handling application commands.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RunAsync()
    {
        var applicationLifetime = _host.Services.GetRequiredService<IHostApplicationLifetime>();
        var pluginService = _host.Services.GetRequiredService<IPluginService>();

        await pluginService.SetupAsync(applicationLifetime.ApplicationStarted);

        await _host.StartAsync();

        await pluginService.StartAsync(applicationLifetime.ApplicationStopping);

        await Task.Delay(Timeout.Infinite, applicationLifetime.ApplicationStopping).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

        await pluginService.StopAsync(applicationLifetime.ApplicationStopped);

        await _host.StopAsync();
    }

    /// <summary>
    /// Disposes the application.
    /// </summary>
    public void Dispose()
    {
        _host.Dispose();
    }

    /// <summary>
    /// Disposes the application asynchronously.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        return ((IAsyncDisposable)_host).DisposeAsync();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="QosmosApplicationBuilder"/> for configuring the application.
    /// </summary>
    /// <returns>A configured instance of <see cref="QosmosApplicationBuilder"/>.</returns>
    public static QosmosApplicationBuilder CreateBuilder()
    {
        return new QosmosApplicationBuilder();
    }
}
