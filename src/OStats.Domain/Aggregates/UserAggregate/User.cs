using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.UserAggregate;


public class User : Entity, IAggregateRoot
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string AuthIdentity { get; private set; }
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public User(string name, string email, string authIdentity)
    {
        Name = name;
        Email = email;
        AuthIdentity = authIdentity;
    }
}