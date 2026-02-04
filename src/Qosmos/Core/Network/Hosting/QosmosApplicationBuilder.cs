// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qosmos.Core.Events.Extensions;
using Qosmos.Core.Extensions;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Qosmos.Core.Network.Hosting;

/// <summary>
/// Provides a builder for configuring and creating a Qosmos application.
/// </summary>
internal sealed class QosmosApplicationBuilder
{
    private readonly HostApplicationBuilder _builder;

    /// <summary>
    /// Gets the configuration manager for the application.
    /// </summary>
    public IConfigurationManager Configuration =>
        _builder.Configuration;

    /// <summary>
    /// Gets the host environment for the application.
    /// </summary>
    public IHostEnvironment Environment =>
        _builder.Environment;

    /// <summary>
    /// Gets the logging builder for configuring logging services.
    /// </summary>
    public ILoggingBuilder Logging =>
        _builder.Logging;

    /// <summary>
    /// Gets the metrics builder for configuring metrics services.
    /// </summary>
    public IMetricsBuilder Metrics =>
        _builder.Metrics;

    /// <summary>
    /// Gets the service collection for registering application services.
    /// </summary>
    public IServiceCollection Services =>
        _builder.Services;

    /// <summary>
    /// Initializes a new instance of the <see cref="QosmosApplicationBuilder"/> class.
    /// </summary>
    public QosmosApplicationBuilder()
    {
        var configuration = new ConfigurationManager();

        configuration
            .AddEnvironmentVariables(prefix: System.Environment.DotnetPrefix)
            .AddEnvironmentVariables(prefix: System.Environment.QosmosPrefix);

        _builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings { Configuration = configuration });

        _builder.ConfigureContainer(GetDefaultServiceProviderFactory(_builder.Environment));

        ConfigureAppConfigurationDefaults(_builder.Configuration, _builder.Environment);
        ConfigureLoggingDefaults(_builder.Logging, _builder.Environment);
        ConfigureServiceDefaults(_builder.Services);
    }

    /// <summary>
    /// Builds and returns a Qosmos application.
    /// </summary>
    /// <returns>A configured instance of <see cref="QosmosApplication"/>.</returns>
    public QosmosApplication Build()
    {
        return new QosmosApplication(_builder.Build());
    }

    /// <summary>
    /// Gets the default service provider factory based on the environment.
    /// </summary>
    /// <param name="environment">The host environment.</param>
    /// <returns>A configured <see cref="DefaultServiceProviderFactory"/>.</returns>
    private static DefaultServiceProviderFactory GetDefaultServiceProviderFactory(IHostEnvironment environment)
    {
        return environment.IsDevelopment()
            ? new DefaultServiceProviderFactory(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true })
            : new DefaultServiceProviderFactory();
    }

    /// <summary>
    /// Configures default application settings for the configuration manager.
    /// </summary>
    /// <param name="configuration">The configuration manager.</param>
    /// <param name="environment">The host environment.</param>
    private static void ConfigureAppConfigurationDefaults(IConfigurationManager configuration, IHostEnvironment environment)
    {
        configuration
            .AddJsonFile("Configurations/Blacklist.json", optional: false, reloadOnChange: true)
            .AddJsonFile("Configurations/Permissions.json", optional: false, reloadOnChange: true)
            .AddJsonFile("Configurations/Settings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("Configurations/Whitelist.json", optional: false, reloadOnChange: true);

        configuration
            .AddJsonFile($"Configurations/Blacklist.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"Configurations/Permissions.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"Configurations/Settings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"Configurations/Whitelist.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    }

    /// <summary>
    /// Configures default logging settings for the application.
    /// </summary>
    /// <param name="logging">The logging builder.</param>
    /// <param name="environment">The host environment.</param>
    private static void ConfigureLoggingDefaults(ILoggingBuilder logging, IHostEnvironment environment)
    {
        var configuration = new LoggerConfiguration();

        if (environment.IsDevelopment())
            configuration.MinimumLevel.Debug();
        else
            configuration.MinimumLevel.Information();

        configuration
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);

        configuration
            .Enrich.WithProperty("Environment", environment.EnvironmentName)
            .Enrich.WithProperty("Application", environment.ApplicationName)
            .Enrich.With<ShortSourceContextEnricher>()
            .Enrich.FromLogContext();

        configuration
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({ShortSourceContext:l}) {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "Logs/Log-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 31,
                fileSizeLimitBytes: 1_073_741_824,
                rollOnFileSizeLimit: true
            );

        logging
            .ClearProviders()
            .AddSerilog(configuration.CreateLogger(), dispose: true);
    }

    /// <summary>
    /// Configures default services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    private static void ConfigureServiceDefaults(IServiceCollection services)
    {
        var commandLineOptions = ParseCommandLineOptions();

        services
            .AddEventBus()
            .AddSingleton<IHostLifetime, QosmosApplicationLifetime>()
            .AddSingleton(Options.Create(commandLineOptions));
    }

    /// <summary>
    /// Parses the command-line arguments into a <see cref="QosmosApplicationOptions"/> instance.
    /// </summary>
    /// <returns>A <see cref="QosmosApplicationOptions"/> instance containing the parsed command-line arguments.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the command-line arguments cannot be parsed.</exception>
    private static QosmosApplicationOptions ParseCommandLineOptions()
    {
        var args = System.Environment
            .GetCommandLineArgs()
            .Skip(1)
            .ToArray();

        QosmosApplicationOptions? options = null;

        Parser.Default
            .ParseArguments<QosmosApplicationOptions>(args)
            .WithParsed(parsed => options = parsed)
            .WithNotParsed(HandleCommandLineErrors);

        return options ?? throw new InvalidOperationException("Failed to parse command line arguments.");
    }

    /// <summary>
    /// Handles errors that occur during the parsing of command-line arguments.
    /// </summary>
    /// <param name="errors">A collection of errors encountered during parsing.</param>
    private static void HandleCommandLineErrors(IEnumerable<Error> errors)
    {
        var isHelpOrVersion = errors.Any(x => x is HelpRequestedError or VersionRequestedError);

        if (!isHelpOrVersion)
        {
            Console.WriteLine();
            Console.WriteLine("Failed to parse command line arguments");
            Console.WriteLine("Use --help for more information");
        }

        System.Environment.Exit(isHelpOrVersion ? 0 : 1);
    }

    /// <summary>
    /// A custom log event enricher that shortens the source context property.
    /// </summary>
    private sealed class ShortSourceContextEnricher : ILogEventEnricher
    {
        /// <summary>
        /// Enriches the log event with a shortened source context property.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">The property factory for creating log event properties.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Properties.TryGetValue("SourceContext", out var sourceContextProperty) && sourceContextProperty is ScalarValue { Value: string sourceContext })
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("ShortSourceContext", sourceContext.Split('.').Last()));
        }
    }
}
