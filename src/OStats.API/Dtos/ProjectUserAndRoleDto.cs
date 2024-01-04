using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public record ProjectUserAndRoleDto
{
    public BaseUserDto User { get; }
    public Role Role { get; }

    public ProjectUserAndRoleDto(User user, Role role)
    {
        User = new BaseUserDto(user);
        Role = role;
    }

    public ProjectUserAndRoleDto(BaseUserDto user, Role role)
    {
        User = user;
        Role = role;
    }
}