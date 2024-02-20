using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public record CreateProjectDto
{
    [Required]
    public required string Title { get; init; }
    public string? Description { get; init; }
}