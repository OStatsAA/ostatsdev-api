namespace OStats.API.Commands;

public sealed record UpdateDatasetVisibilityCommand
{
    public required Guid DatasetId { get; init; }
    public required string UserAuthId { get; init; }
    public required bool IsPublic { get; init; }
}
