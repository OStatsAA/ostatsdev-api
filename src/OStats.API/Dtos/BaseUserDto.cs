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
}