using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public record UpdateProjectDto : CreateProjectDto
{
    [Required]
    public required DateTime LastUpdatedAt { get; init; }
}