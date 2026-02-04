// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Qosmos.Core.Events.Abstractions;

namespace Qosmos.Core.Events.Extensions;

/// <summary>
/// Provides extension methods for registering event bus services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the event bus and its dependencies to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to add the event bus to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        return services.AddSingleton<IEventBus, EventBus>();
    }
}
