using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.UserAggregate.Events;

public sealed record DeletedUserDomainEvent : IDomainEvent
{
    public required Guid UserId { get; init; }
    public required Guid RequestorId { get; init; }
}