// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Qosmos.Core.Events.Abstractions;

namespace Qosmos.Core.Events;

/// <summary>
/// Represents an implementation of the <see cref="IEventBus"/> interface for publishing events
/// to registered event handlers using dependency injection.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBus"/> class with the specified service provider.
    /// </summary>
    /// <param name="provider">The service provider used to resolve event handlers.</param>
    public EventBus(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Publishes an event asynchronously to all registered handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to publish. Must implement <see cref="IEvent"/>.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation of publishing the event.</returns>
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        var tasks = _provider
            .GetServices<IEventHandler<TEvent>>()
            .Select(handler => handler.HandleAsync(@event, cancellationToken));

        return Task.WhenAll(tasks);
    }
}
