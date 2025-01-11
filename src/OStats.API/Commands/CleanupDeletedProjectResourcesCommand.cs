namespace OStats.API.Commands;
public sealed record CleanupDeletedProjectResourcesCommand
{
    public required Guid ProjectId { get; init; }
}