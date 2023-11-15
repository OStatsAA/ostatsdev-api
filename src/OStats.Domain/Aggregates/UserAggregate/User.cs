using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.UserAggregate;


public class User : Entity, IAggregateRoot
{
    public User(string name, string email, string authIdentity)
    {
        Name = name;
        Email = email;
        AuthIdentity = authIdentity;
    }

    public string Name { get; private set; }
    public string Email { get; private set; }
    public string AuthIdentity { get; private set; }

}