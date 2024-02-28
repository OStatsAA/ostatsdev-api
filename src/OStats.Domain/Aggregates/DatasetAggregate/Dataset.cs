using System.Data;
using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.DatasetAggregate;

public class Dataset : Entity, IAggregateRoot
{
    public string Title { get; set; }
    public string Source { get; set; }
    public string? Description { get; set; }
    private readonly List<DatasetUserAccessLevel> _datasetUsersAccessesLevels = new List<DatasetUserAccessLevel>();
    public IReadOnlyCollection<DatasetUserAccessLevel> DatasetUserAccessLevels => _datasetUsersAccessesLevels.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Dataset(string title, string source)
    {
        Title = title;
        Source = source;
    }

    public Dataset(Guid ownerId, string title, string source)
    {
        Title = title;
        Source = source;
        _datasetUsersAccessesLevels.Add(new DatasetUserAccessLevel(Id, ownerId, DatasetAccessLevel.Owner));
    }

    public Dataset(Guid ownerId, string title, string source, string description)
    {
        Title = title;
        Source = source;
        Description = description;
        _datasetUsersAccessesLevels.Add(new DatasetUserAccessLevel(Id, ownerId, DatasetAccessLevel.Owner));
    }

    public DomainOperationResult GrantUserAccess(Guid userId, DatasetAccessLevel accessLevel, Guid requestorId)
    {
        var requestorAccessLevel = GetUserAccessLevel(requestorId);
        if (requestorAccessLevel < DatasetAccessLevel.Administrator)
        {
            return DomainOperationResult.Failure("Requestor does not have permission to grant user access.");
        }

        var datasetUserAccess = new DatasetUserAccessLevel(Id, userId, accessLevel);
        _datasetUsersAccessesLevels.Add(datasetUserAccess);
        _domainEvents.Add(new GrantedUserAccessToDatasetDomainEvent(datasetUserAccess, requestorId));
        return DomainOperationResult.Success;
    }

    public DomainOperationResult RemoveUserAccess(Guid userId, Guid requestorId)
    {
        var requestorAccessLevel = GetUserAccessLevel(requestorId);
        if (requestorAccessLevel < DatasetAccessLevel.Administrator)
        {
            return DomainOperationResult.Failure("Requestor does not have permission to remove user access.");
        }

        var userAccess = _datasetUsersAccessesLevels.SingleOrDefault(userAccess => userAccess.UserId == userId);
        if (userAccess is null)
        {
            return DomainOperationResult.Failure("User does not have access to this dataset.");
        }

        if (userAccess.AccessLevel > requestorAccessLevel)
        {
            return DomainOperationResult.Failure("Requestor does not have permission to remove user access.");
        }

        _datasetUsersAccessesLevels.Remove(userAccess);
        _domainEvents.Add(new RevokedUserAccessFromDatasetDomainEvent(userAccess, requestorId));
        return DomainOperationResult.Success;
    }

    public DatasetAccessLevel GetUserAccessLevel(Guid userId)
    {
        return _datasetUsersAccessesLevels
            .Where(userAccess => userAccess.UserId == userId)
            .Select(userAccess => userAccess.AccessLevel)
            .SingleOrDefault(DatasetAccessLevel.NoAccess);
    }
}