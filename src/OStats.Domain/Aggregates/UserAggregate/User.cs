using OStats.Domain.Aggregates.UserAggregate.Events;
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

    public DomainOperationResult Delete(Guid requestorId)
    {
        if (IsDeleted)
        {
            return DomainOperationResult.NoActionTaken("User is already deleted");
        }

        if (requestorId != Id)
        {
            return DomainOperationResult.Unauthorized("Only the user can delete their account");
        }

        _domainEvents.Add(new DeletedUserDomainEvent { UserId = Id, RequestorId = requestorId });
        IsDeleted = true;
        return DomainOperationResult.Success;
    }
}