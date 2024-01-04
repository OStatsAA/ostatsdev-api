namespace OStats.API.Dtos;

public record UpdateProjectDto : CreateProjectDto
{
    public required DateTime LastUpdatedAt { get; init; }
}