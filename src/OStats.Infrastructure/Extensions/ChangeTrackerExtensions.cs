using Microsoft.EntityFrameworkCore.ChangeTracking;
using OStats.Domain.Common;

namespace OStats.Infrastructure.Extensions;
public static class ChangeTrackerExtensions
{
    public static IEnumerable<IDomainEvent>? GetAggregateRootDomainEvents(this ChangeTracker changeTracker)
    {
        if (changeTracker.HasChanges())
        {
            return changeTracker
                .Entries<IAggregateRoot>()
                .Where(entry => entry.Entity.DomainEvents.Count != 0)
                .SelectMany(entry => entry.Entity.DomainEvents);
        }

        return null;
    }
}