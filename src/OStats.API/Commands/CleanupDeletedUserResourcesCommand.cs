namespace OStats.API.Commands;
public sealed record CleanupDeletedUserResourcesCommand
{
    public required Guid UserId { get; init; }
}