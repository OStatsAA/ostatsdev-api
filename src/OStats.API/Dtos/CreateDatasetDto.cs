using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public record CreateDatasetDto
{
    [Required]
    public required string Title { get; init; }

    [Required]
    public required string Source { get; init; }
    public string? Description { get; init; }
}