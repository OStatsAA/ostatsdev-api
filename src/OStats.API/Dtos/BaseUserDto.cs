using System.Text.Json.Serialization;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public record BaseUserDto : BaseEntityDto
{
    public string Name { get; }
    public string Email { get; }

    public BaseUserDto(User user) : base(user)
    {
        Name = user.Name;
        Email = user.Email;
    }

    [JsonConstructor]
    public BaseUserDto(Guid id, string name, string email, DateTime createdAt, DateTime lastUpdatedAt) : base(id, createdAt, lastUpdatedAt)
    {
        Name = name;
        Email = email;
    }
}