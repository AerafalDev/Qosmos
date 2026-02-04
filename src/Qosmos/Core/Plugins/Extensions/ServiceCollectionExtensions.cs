// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;

namespace Qosmos.Core.Plugins.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection"/> to add plugins.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a plugin to the service collection with its manifest and marks it as waiting to load.
    /// </summary>
    /// <typeparam name="T">The type of the plugin, which must inherit from <see cref="Plugin"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the plugin will be added.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with the plugin registered as waiting to load.
    /// </returns>
    public static IServiceCollection AddPlugin<T>(this IServiceCollection services)
        where T : Plugin
    {
        return services.AddSingleton(new PluginWaitingToLoad(string.Empty, PluginManifest.CorePlugin<T>().Build(), true));
    }
}
