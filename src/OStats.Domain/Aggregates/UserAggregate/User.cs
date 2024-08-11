using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.UserAggregate;


public sealed class User : AggregateRoot
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string AuthIdentity { get; private set; }
    public User(string name, string email, string authIdentity)
    {
        Name = name;
        Email = email;
        AuthIdentity = authIdentity;
    }
}