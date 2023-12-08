using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public class BaseUserDto
{
    public Guid Id { get; }
    public string Name { get; }
    public string Email { get; }
    public BaseUserDto(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Email = user.Email;
    }
}