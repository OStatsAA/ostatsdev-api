namespace OStats.API.Dtos;

public record CreateDatasetDto
{
    public required string Title { get; init; }
    public required string Source { get; init; }
    public string? Description { get; init; }
}