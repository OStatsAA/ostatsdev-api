using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public record UpdateDatasetDto : CreateDatasetDto
{
    [Required]
    public required DateTime LastUpdatedAt { get; init; }
}