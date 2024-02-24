using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public record CreateDatasetDto
{
    [Required]
    [Length(1, 256)]
    public required string Title { get; init; }

    [Required]
    [Length(1, 256)]
    public required string Source { get; init; }

    [MaxLength(4096)]
    public string? Description { get; init; }
}