namespace OStats.API.Commands;

public sealed record UpdateProjectCommand : CreateProjectCommand
{
    public Guid Id { get; init; }
    public DateTime LastUpdatedAt { get; init; }

    public UpdateProjectCommand(Guid id, Guid requestorUserId, string title, DateTime lastUpdatedAt, string? description) : base(requestorUserId, title, description)
    {
        Id = id;
        LastUpdatedAt = lastUpdatedAt;
    }
}