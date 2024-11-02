namespace OStats.Domain.Common;

public abstract class AggregateRoot : Entity
{
    private protected readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
}