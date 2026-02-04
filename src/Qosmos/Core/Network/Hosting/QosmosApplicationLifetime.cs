// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Qosmos.Core.Network.Hosting;

/// <summary>
/// Represents a custom implementation of <see cref="IHostLifetime"/> for managing the lifetime.
/// </summary>
internal sealed class QosmosApplicationLifetime : IHostLifetime, IDisposable
{
    private static readonly Assembly s_assembly = typeof(QosmosApplicationLifetime).Assembly;

    private static readonly string? s_version = s_assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

    private static readonly string? s_framework = s_assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkDisplayName;

    private static readonly ConsoleColor[] s_consoleColors =
    [
        ConsoleColor.Green,
        ConsoleColor.Red,
        ConsoleColor.Magenta,
        ConsoleColor.Yellow
    ];

    private static readonly string[] s_asciiLogo =
    [
        " ██████╗  ██████╗ ███████╗███╗   ███╗ ██████╗ ███████╗",
        "██╔═══██╗██╔═══██╗██╔════╝████╗ ████║██╔═══██╗██╔════╝",
        "██║   ██║██║   ██║███████╗██╔████╔██║██║   ██║███████╗",
        "██║▄▄ ██║██║   ██║╚════██║██║╚██╔╝██║██║   ██║╚════██║",
        "╚██████╔╝╚██████╔╝███████║██║ ╚═╝ ██║╚██████╔╝███████║",
        " ╚══▀▀═╝  ╚═════╝ ╚══════╝╚═╝     ╚═╝ ╚═════╝ ╚══════╝"
    ];

    private readonly IHostEnvironment _environment;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly HostOptions _hostOptions;

    private CancellationTokenRegistration _applicationStartedRegistration;

    private PosixSignalRegistration? _sigIntRegistration;
    private PosixSignalRegistration? _sigQuitRegistration;
    private PosixSignalRegistration? _sigTermRegistration;

    /// <summary>
    /// Initializes a new instance of the <see cref="QosmosApplicationLifetime"/> class.
    /// </summary>
    /// <param name="environment">The host environment.</param>
    /// <param name="applicationLifetime">The application lifetime.</param>
    /// <param name="hostOptions">The host options.</param>
    public QosmosApplicationLifetime(IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, IOptions<HostOptions> hostOptions)
    {
        _environment = environment;
        _applicationLifetime = applicationLifetime;
        _hostOptions = hostOptions.Value;
    }

    /// <summary>
    /// Waits for the application to start and registers signal handlers.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A completed task.</returns>
    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        _applicationStartedRegistration = _applicationLifetime.ApplicationStarted.Register(state => ((QosmosApplicationLifetime)state!).OnApplicationStarted(), this);

        _sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, HandlePosixSignal);
        _sigQuitRegistration = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, HandlePosixSignal);
        _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, OperatingSystem.IsWindows() ? HandleWindowsShutdown : HandlePosixSignal);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the application gracefully.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A completed task.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes resources used by the application lifetime.
    /// </summary>
    public void Dispose()
    {
        _sigIntRegistration?.Dispose();
        _sigQuitRegistration?.Dispose();
        _sigTermRegistration?.Dispose();
        _applicationStartedRegistration.Dispose();
    }

    /// <summary>
    /// Handles the application startup logic, including displaying the ASCII logo and application metadata.
    /// </summary>
    private void OnApplicationStarted()
    {
        Console.WriteLine();
        Console.ForegroundColor = s_consoleColors[Random.Shared.Next(s_consoleColors.Length)];

        foreach (var row in s_asciiLogo)
            Console.WriteLine(row);

        Console.WriteLine();
        Console.WriteLine("Application started. Press Ctrl+C to shut down");
        Console.WriteLine();
        Console.WriteLine("Application Name: {0}", _environment.ApplicationName);
        Console.WriteLine("Application Version: {0}", s_version);
        Console.WriteLine("Application Framework: {0}", s_framework);
        Console.WriteLine("Application Environment: {0}", _environment.EnvironmentName);
        Console.WriteLine("Application Root Path: {0}", _environment.ContentRootPath);
        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>
    /// Handles POSIX signals to stop the application gracefully.
    /// </summary>
    /// <param name="context">The POSIX signal context.</param>
    private void HandlePosixSignal(PosixSignalContext context)
    {
        context.Cancel = true;
        _applicationLifetime.StopApplication();
    }

    /// <summary>
    /// Handles the SIGTERM signal on Windows, ensuring the application shuts down gracefully.
    /// </summary>
    /// <param name="context">The POSIX signal context.</param>
    private void HandleWindowsShutdown(PosixSignalContext context)
    {
        // For SIGTERM on Windows, block this thread until the application is finished.
        // This prevents the process from being killed immediately on return from this handler.
        // Don't allow Dispose to unregister handlers, since Windows has a lock that prevents unregistration while this handler is running.
        // Just leak these, since the process is exiting.
        _sigIntRegistration = null;
        _sigQuitRegistration = null;
        _sigTermRegistration = null;

        _applicationLifetime.StopApplication();

        Thread.Sleep(_hostOptions.ShutdownTimeout);
    }
}
