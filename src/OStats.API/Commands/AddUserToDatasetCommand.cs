using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.API.Commands;

public sealed record AddUserToDatasetCommand
{
    public Guid RequestorUserId { get; set; }
    public Guid DatasetId { get; set; }
    public Guid UserId { get; set; }
    public DatasetAccessLevel AccessLevel { get; set; }

    public AddUserToDatasetCommand(Guid requestorUserId, Guid datasetId, Guid userId, DatasetAccessLevel accessLevel)
    {
        RequestorUserId = requestorUserId;
        UserId = userId;
        DatasetId = datasetId;
        AccessLevel = accessLevel;
    }
}