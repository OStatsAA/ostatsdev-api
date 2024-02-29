using MassTransit;
using OStats.Domain.Common;

namespace OStats.API.Extensions;

public static class PublishEndpointExtensions
{
    /// <summary>
    /// Publishes the domain events of an aggregate root asynchronously to Outbox.
    /// Message gets pushed to message broker after EF Core transaction is committed.
    /// ref: https://masstransit.io/documentation/configuration/middleware/outbox
    /// </summary>
    /// <param name="publishEndpoint">The publish endpoint.</param>
    /// <param name="aggregateRoot">The aggregate root.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task PublishDomainEventsAsync(this IPublishEndpoint publishEndpoint, IAggregateRoot aggregateRoot, CancellationToken cancellationToken)
    {
        if (!aggregateRoot.DomainEvents.Any())
        {
            return;
        }

        foreach (var domainEvent in aggregateRoot.DomainEvents)
        {
            await publishEndpoint.Publish(domainEvent, domainEvent.GetType(), cancellationToken);
        }
    }
}
