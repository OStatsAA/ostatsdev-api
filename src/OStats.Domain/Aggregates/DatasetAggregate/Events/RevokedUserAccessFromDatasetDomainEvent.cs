using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.DatasetAggregate;

public sealed class RevokedUserAccessFromDatasetDomainEvent : IDomainEvent
{
    public DatasetUserAccessLevel DatasetUserAccessLevel { get; init; }
    public Guid RequestorId { get; init; }

    public RevokedUserAccessFromDatasetDomainEvent(DatasetUserAccessLevel datasetUserAccessLevel, Guid requestorId)
    {
        DatasetUserAccessLevel = datasetUserAccessLevel;
        RequestorId = requestorId;
    }
}
