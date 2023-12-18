using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.DatasetAggregate;

public class DatasetUserAccessLevel : Entity
{
    public Guid DatasetId { get; }
    public Guid UserId { get; }
    public DatasetAccessLevel AccessLevel { get; set; }

    public DatasetUserAccessLevel(Guid datasetId, Guid userId, DatasetAccessLevel accessLevel)
    {
        DatasetId = datasetId;
        UserId = userId;
        AccessLevel = accessLevel;
    }
}