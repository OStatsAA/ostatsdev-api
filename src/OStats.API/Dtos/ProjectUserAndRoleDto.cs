using System.Text.Json.Serialization;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public sealed record ProjectUserAndRoleDto
{
    public BaseUserDto User { get; init; }
    public AccessLevel AccessLevel { get; init; }

    public ProjectUserAndRoleDto(User user, AccessLevel accessLevel)
    {
        User = new BaseUserDto(user);
        AccessLevel = accessLevel;
    }

    [JsonConstructor]
    public ProjectUserAndRoleDto(BaseUserDto user, AccessLevel accessLevel)
    {
        User = user;
        AccessLevel = accessLevel;
    }
}