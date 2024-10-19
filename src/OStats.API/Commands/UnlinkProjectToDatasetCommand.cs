namespace OStats.API.Commands;

public sealed record UnlinkProjectToDatasetCommand
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }
    public Guid ProjectId { get; }
    public UnlinkProjectToDatasetCommand(string userAuthId, Guid datasetId, Guid projectId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
        ProjectId = projectId;
    }
}