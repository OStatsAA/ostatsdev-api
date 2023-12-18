namespace OStats.API.Dtos;

public record UpdateDatasetCommandDto : CreateDatasetCommandDto
{
    public required DateTime LastUpdatedAt { get; init; }
}