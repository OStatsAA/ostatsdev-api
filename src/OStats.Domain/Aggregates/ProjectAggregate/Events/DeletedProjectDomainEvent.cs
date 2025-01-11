using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.ProjectAggregate.Events;

public sealed record DeletedProjectDomainEvent : IDomainEvent
{
    public required Guid ProjectId { get; init; }
    public required Guid RequestorId { get; init; }
}