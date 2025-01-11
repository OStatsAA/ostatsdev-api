using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.DatasetAggregate.Events;

public sealed record DeletedDatasetDomainEvent : IDomainEvent
{
    public required Guid DatasetId { get; init; }
    public required Guid RequestorId { get; init; }
}