namespace OStats.API.Commands;

public sealed record LinkProjectToDatasetCommand
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }
    public Guid ProjectId { get; }
    public LinkProjectToDatasetCommand(string userAuthId, Guid datasetId, Guid projectId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
        ProjectId = projectId;
    }
}