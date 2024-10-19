namespace OStats.API.Commands;

public sealed record DeleteDatasetCommand
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }

    public DeleteDatasetCommand(string userAuthId, Guid datasetId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
    }
}