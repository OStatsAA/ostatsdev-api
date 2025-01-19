namespace OStats.API.Commands;

public sealed record UpdateDatasetCommand : CreateDatasetCommand
{
    public Guid Id { get; init; }
    public DateTime LastUpdatedAt { get; init; }

    public UpdateDatasetCommand(Guid id, Guid requestorUserId, string title, string source, DateTime lastUpdatedAt, string? description) : base(requestorUserId, title, source, description)
    {
        Id = id;
        LastUpdatedAt = lastUpdatedAt;
    }
}