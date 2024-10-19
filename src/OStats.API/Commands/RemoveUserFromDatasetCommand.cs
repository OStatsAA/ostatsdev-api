namespace OStats.API.Commands;

public sealed record RemoveUserFromDatasetCommand
{
    public string UserAuthId { get; set; }
    public Guid DatasetId { get; set; }
    public Guid UserId { get; set; }

    public RemoveUserFromDatasetCommand(string userAuthId, Guid datasetId, Guid userId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
        UserId = userId;
    }
}