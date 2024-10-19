namespace OStats.API.Commands;

public sealed record UpdateDatasetCommand : CreateDatasetCommand
{
    public Guid Id { get; init; }
    public DateTime LastUpdatedAt { get; init; }

    public UpdateDatasetCommand(Guid id, string userAuthId, string title, string source, DateTime lastUpdatedAt, string? description) : base(userAuthId, title, source, description)
    {
        Id = id;
        LastUpdatedAt = lastUpdatedAt;
    }
}