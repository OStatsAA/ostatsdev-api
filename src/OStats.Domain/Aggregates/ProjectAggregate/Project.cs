using OStats.Domain.Common;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;

namespace OStats.Domain.Aggregates.ProjectAggregate;

public class Project : Entity, IAggregateRoot
{
    public string Title { get; set; }
    public string? Description { get; set; }
    private readonly List<Role> _roles = new List<Role>();
    public IReadOnlyCollection<Role> Roles => _roles;
    private readonly HashSet<DatasetProjectLink> _linkedDatasets = new HashSet<DatasetProjectLink>();
    public IReadOnlyCollection<DatasetProjectLink> LinkedDatasets => _linkedDatasets;

    private Project(string title, string? description = null)
    {
        Title = title;
        Description = description;
    }

    public Project(Guid ownerId, string title, string? description = null)
    {
        Title = title;
        Description = description;
        _roles.Add(new Role(Id, ownerId, AccessLevel.Owner));
    }

    public DomainOperationResult AddOrUpdateUserRole(Guid userId, AccessLevel accessLevel, Guid requestorId)
    {
        var requestorRole = _roles.GetUserRole(requestorId);
        if (requestorRole is null || requestorRole.AccessLevel < AccessLevel.Administrator)
        {
            return DomainOperationResult.Failure("Requestor does not have permission to add or update user role.");
        }

        var userRole = _roles.GetUserRole(userId);
        if (userRole != null)
        {
            userRole.AccessLevel = accessLevel;
        }
        else
        {
            _roles.Add(new Role(Id, userId, accessLevel));
        }

        return DomainOperationResult.Success;
    }

    public DomainOperationResult RemoveUserRole(Guid userId, Guid requestorId)
    {
        var requestorRole = _roles.GetUserRole(requestorId);
        if (requestorRole is null || requestorRole.AccessLevel < AccessLevel.Administrator)
        {
            return DomainOperationResult.Failure("Requestor does not have permission to remove user role.");
        }

        var userRole = _roles.GetUserRole(userId);
        if (userRole is null)
        {
            return DomainOperationResult.Failure("User does not have a role in this project.");
        }

        _roles.Remove(userRole);
        return DomainOperationResult.Success;
    }

    public Role? GetUserRole(Guid userId)
    {
        return _roles.Find(role => role.UserId == userId);
    }

    public DomainOperationResult LinkDataset(Guid datasetId, Guid requestorId)
    {
        var requestorRole = _roles.GetUserRole(requestorId);
        if (requestorRole is null || requestorRole.AccessLevel < AccessLevel.Editor)
        {
            return DomainOperationResult.Failure("Requestor does not have permission to link dataset.");
        }

        if (_linkedDatasets.Any(link => link.DatasetId == datasetId))
        {
            return DomainOperationResult.Failure("Dataset is already linked to this project.");
        }

        _linkedDatasets.Add(new DatasetProjectLink(datasetId, Id));
        return DomainOperationResult.Success;
    }

    public DomainOperationResult UnlinkDataset(Guid datasetId, Guid requestorId)
    {
        var requestorRole = _roles.GetUserRole(requestorId);
        if (requestorRole is null || requestorRole.AccessLevel < AccessLevel.Editor)
        {
            return DomainOperationResult.Failure("Requestor does not have permission to unlink dataset.");
        }

        var removed = _linkedDatasets.RemoveWhere(link => link.DatasetId == datasetId) > 0;
        if (removed)
        {
            return DomainOperationResult.Success;
        }
        else
        {
            return DomainOperationResult.Failure("Dataset is not linked to this project.");
        }
    }
}