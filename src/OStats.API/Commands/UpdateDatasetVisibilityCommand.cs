namespace OStats.API.Commands;

public sealed record UpdateDatasetVisibilityCommand
{
    public required Guid DatasetId { get; init; }
    public required Guid RequestorUserId { get; init; }
    public required bool IsPublic { get; init; }
}
