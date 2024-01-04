namespace OStats.API.Dtos;

public record UpdateDatasetDto : CreateDatasetDto
{
    public required DateTime LastUpdatedAt { get; init; }
}