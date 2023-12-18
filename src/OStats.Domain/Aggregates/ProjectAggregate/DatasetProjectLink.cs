using System.Collections;
using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.ProjectAggregate;

public class DatasetProjectLink : Entity
{
    public Guid DatasetId { get; }
    public Guid ProjectId { get; }

    public DatasetProjectLink(Guid datasetId, Guid projectId)
    {
        DatasetId = datasetId;
        ProjectId = projectId;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}