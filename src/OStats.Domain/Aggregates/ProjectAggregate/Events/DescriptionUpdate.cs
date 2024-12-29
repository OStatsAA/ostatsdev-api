using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.ProjectAggregate.Events;
public sealed record DescriptionUpdate : IDomainEvent
{
    public required Guid ProjectId {get; init;}
    public required Guid RequestorId {get; init;}
    public required string OldDescription {get; init;}
    public required string Description {get; init;}
}