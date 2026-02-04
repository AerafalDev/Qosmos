// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qosmos.Core.Events.Abstractions;

/// <summary>
/// Represents an event bus for publishing events.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event asynchronously to all registered handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to publish. Must implement <see cref="IEvent"/>.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;
}
