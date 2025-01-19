namespace OStats.API.Commands;

public sealed record UnlinkProjectToDatasetCommand
{
    public Guid RequestorUserId { get; }
    public Guid DatasetId { get; }
    public Guid ProjectId { get; }
    public UnlinkProjectToDatasetCommand(Guid requestorUserId, Guid datasetId, Guid projectId)
    {
        RequestorUserId = requestorUserId;
        DatasetId = datasetId;
        ProjectId = projectId;
    }
}