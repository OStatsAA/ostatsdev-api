using System.Text.Json.Serialization;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public record ProjectUserAndRoleDto
{
    public BaseUserDto User { get; init; }
    public Role Role { get; init; }

    public ProjectUserAndRoleDto(User user, Role role)
    {
        User = new BaseUserDto(user);
        Role = role;
    }

    [JsonConstructor]
    public ProjectUserAndRoleDto(BaseUserDto user, Role role)
    {
        User = user;
        Role = role;
    }

}