using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.ProjectAggregate.Events;
public sealed record TitleUpdateDomainEvent : IDomainEvent
{
    public required Guid ProjectId { get; init; }
    public required Guid RequestorId { get; init; }
    public required string OldTitle { get; init; }
    public required string Title { get; init; }
}