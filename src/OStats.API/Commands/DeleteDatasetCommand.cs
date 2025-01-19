namespace OStats.API.Commands;

public sealed record DeleteDatasetCommand
{
    public Guid RequestorUserId { get; }
    public Guid DatasetId { get; }

    public DeleteDatasetCommand(Guid requestorUserId, Guid datasetId)
    {
        RequestorUserId = requestorUserId;
        DatasetId = datasetId;
    }
}