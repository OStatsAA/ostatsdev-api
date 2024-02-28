using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.DatasetAggregate;

public record GrantedUserAccessToDatasetDomainEvent : IDomainEvent
{
    public DatasetUserAccessLevel DatasetUserAccessLevel { get; init; }
    public Guid RequestorId { get; init; }

    public GrantedUserAccessToDatasetDomainEvent(DatasetUserAccessLevel datasetUserAccessLevel, Guid requestorId)
    {
        DatasetUserAccessLevel = datasetUserAccessLevel;
        RequestorId = requestorId;
    }
}