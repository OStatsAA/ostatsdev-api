namespace OStats.API.Commands;

public sealed record RemoveUserFromDatasetCommand
{
    public Guid RequestorUserId { get; set; }
    public Guid DatasetId { get; set; }
    public Guid UserId { get; set; }

    public RemoveUserFromDatasetCommand(Guid requestorUserId, Guid datasetId, Guid userId)
    {
        RequestorUserId = requestorUserId;
        DatasetId = datasetId;
        UserId = userId;
    }
}