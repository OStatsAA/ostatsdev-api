using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public struct CreateUserDto
{
    [Required(AllowEmptyStrings = false)]
    [Length(1, 128)]
    public required string Name { get; init; }

    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    [MaxLength(128)]
    public required string Email { get; init; }
}