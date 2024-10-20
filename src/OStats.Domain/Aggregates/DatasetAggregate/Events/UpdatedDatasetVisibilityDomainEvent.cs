using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.DatasetAggregate.Events;

public sealed record UpdatedDatasetVisibilityDomainEvent(
    Guid DatasetId,
    bool IsPublic,
    Guid UserId) : IDomainEvent
{
}