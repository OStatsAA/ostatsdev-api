using Microsoft.EntityFrameworkCore.ChangeTracking;
using OStats.Domain.Common;

namespace OStats.Infrastructure.Extensions;
public static class ChangeTrackerExtensions
{
    public static IEnumerable<IDomainEvent> GetAggregateRootDomainEvents(this ChangeTracker changeTracker)
    {
        return changeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Count != 0)
            .SelectMany(entry => entry.Entity.DomainEvents ?? Enumerable.Empty<IDomainEvent>());
    }
}