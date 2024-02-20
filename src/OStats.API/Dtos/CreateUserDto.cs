using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public struct CreateUserDto
{
    [Required]
    public required string Name { get; init; }

    [Required]
    [EmailAddress]
    public required string Email { get; init; }
}