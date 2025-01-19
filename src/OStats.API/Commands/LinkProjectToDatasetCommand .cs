namespace OStats.API.Commands;

public sealed record LinkProjectToDatasetCommand
{
    public Guid RequestorUserId { get; }
    public Guid DatasetId { get; }
    public Guid ProjectId { get; }
    public LinkProjectToDatasetCommand(Guid requestorUserId, Guid datasetId, Guid projectId)
    {
        RequestorUserId = requestorUserId;
        DatasetId = datasetId;
        ProjectId = projectId;
    }
}