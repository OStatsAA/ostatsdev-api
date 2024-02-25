using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public record CreateUserDto
{
    [Required]
    [Length(1, 128)]
    public required string Name { get; init; }

    [Required]
    [EmailAddress]
    [MaxLength(128)]
    public required string Email { get; init; }
}