namespace OStats.API.Dtos;

public record CreateProjectCommandDto
{
    public required string Title { get; init; }
    public string? Description { get; init; }
}