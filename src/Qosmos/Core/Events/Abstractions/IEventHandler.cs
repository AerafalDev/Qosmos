// Copyright (c) Qosmos 2026.
// Licensed under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Qosmos.Core.Events.Abstractions;

/// <summary>
/// Defines a handler for processing events of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of the event to handle. Must implement <see cref="IEvent"/>.</typeparam>
public interface IEventHandler<in TEvent>
    where TEvent : class, IEvent
{
    /// <summary>
    /// Handles the specified event asynchronously.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
