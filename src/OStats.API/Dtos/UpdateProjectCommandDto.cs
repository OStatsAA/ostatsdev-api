namespace OStats.API.Dtos;

public record UpdateProjectCommandDto : CreateProjectCommandDto
{
    public required DateTime LastUpdatedAt { get; init; }
}