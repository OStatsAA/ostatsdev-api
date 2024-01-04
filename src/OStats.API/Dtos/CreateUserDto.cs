using System.ComponentModel.DataAnnotations;

namespace OStats.API.Dtos;

public struct CreateUserDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
}