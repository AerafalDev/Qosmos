// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Qosmos.Core.Network.Hosting;

/// <summary>
/// Represents the main application class for a Qosmos application.
/// </summary>
internal sealed class QosmosApplication
{
    private readonly IHost _host;

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
        await _host.StartAsync();

        var applicationLifetime = _host.Services.GetRequiredService<IHostApplicationLifetime>();

        _ = Task.Run(async () =>
        {
            while (!applicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                try
                {
                    var line = await Console.In.ReadLineAsync(applicationLifetime.ApplicationStopping);

                    if (line is null || applicationLifetime.ApplicationStopping.IsCancellationRequested)
                        break;

                    // TODO: Commands
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }, applicationLifetime.ApplicationStopping);

        await _host.WaitForShutdownAsync();
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
