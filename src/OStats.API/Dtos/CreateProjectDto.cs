using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public record CreateProjectDto
{
    [Required(AllowEmptyStrings = false)]
    [Length(1, 256)]
    public required string Title { get; init; }

    [MaxLength(2048)]
    public string? Description { get; init; }
}