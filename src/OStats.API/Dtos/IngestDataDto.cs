using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public record IngestDataDto
{
    [Required]
    public required string Bucket { get; init; }
    [Required]
    public required string FileName { get; init; }
}