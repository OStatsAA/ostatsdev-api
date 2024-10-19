namespace OStats.API.Commands;

public sealed record UpdateProjectCommand : CreateProjectCommand
{
    public Guid Id { get; init; }
    public DateTime LastUpdatedAt { get; init; }

    public UpdateProjectCommand(Guid id, string userAuthId, string title, DateTime lastUpdatedAt, string? description) : base(userAuthId, title, description)
    {
        Id = id;
        LastUpdatedAt = lastUpdatedAt;
    }
}