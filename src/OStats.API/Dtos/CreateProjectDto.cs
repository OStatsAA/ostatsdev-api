namespace OStats.API.Dtos;

public record CreateProjectDto
{
    public required string Title { get; init; }
    public string? Description { get; init; }
}