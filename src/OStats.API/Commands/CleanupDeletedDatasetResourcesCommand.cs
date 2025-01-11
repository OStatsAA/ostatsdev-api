namespace OStats.API.Commands;
public sealed record CleanupDeletedDatasetResourcesCommand
{
    public required Guid DatasetId { get; init; }
}