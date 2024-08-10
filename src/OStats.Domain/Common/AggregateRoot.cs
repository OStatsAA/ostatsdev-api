namespace OStats.Domain.Common;

public abstract class AggregateRoot : Entity
{
    private protected readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
}