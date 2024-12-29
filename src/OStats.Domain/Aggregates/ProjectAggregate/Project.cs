using OStats.Domain.Common;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Domain.Aggregates.ProjectAggregate.Events;

namespace OStats.Domain.Aggregates.ProjectAggregate;

public sealed class Project : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; } = string.Empty;
    private readonly List<Role> _roles = [];
    public IReadOnlyCollection<Role> Roles => _roles;
    private readonly HashSet<DatasetProjectLink> _linkedDatasets = [];
    public IReadOnlyCollection<DatasetProjectLink> LinkedDatasets => _linkedDatasets;

    private Project(string title)
    {
        Title = title;
    }

    public Project(Guid ownerId, string title)
    {
        Title = title;
        _roles.Add(new Role(Id, ownerId, AccessLevel.Owner));
    }

    public Project(Guid ownerId, string title, string description)
    {
        Title = title;
        Description = description;
        _roles.Add(new Role(Id, ownerId, AccessLevel.Owner));
    }

    public DomainOperationResult IsAllowedTo(Role requestorRole, AccessLevel requiredAccessLevel)
    {
        if (requestorRole is null)
        {
            return DomainOperationResult.InvalidUserRole();
        }

        return requestorRole switch
        {
            { ProjectId: var projectId, AccessLevel: var accessLevel } when projectId == Id && accessLevel >= requiredAccessLevel => DomainOperationResult.Success,
            { ProjectId: var projectId } when projectId != Id => DomainOperationResult.InvalidUserRole(),
            { AccessLevel: var accessLevel } when accessLevel < requiredAccessLevel => DomainOperationResult.Unauthorized(),
            _ => DomainOperationResult.InvalidUserRole()
        };
    }

    public DomainOperationResult SetTitle(string title, Role requestorRole)
    {
        if (IsAllowedTo(requestorRole, AccessLevel.Editor) is var result && !result.Succeeded)
        {
            return result;
        }

        _domainEvents.Add(new TitleUpdate { ProjectId = Id, RequestorId = requestorRole.UserId, OldTitle = Title, Title = title });
        Title = title;
        return result;
    }

    public DomainOperationResult SetDescription(string description, Role requestorRole)
    {
        if (IsAllowedTo(requestorRole, AccessLevel.Editor) is var result && !result.Succeeded)
        {
            return result;
        }

        _domainEvents.Add(new DescriptionUpdate { ProjectId = Id, RequestorId = requestorRole.UserId, OldDescription = Description, Description = description });
        Description = description;
        return result;
    }

    public DomainOperationResult AddOrUpdateUserRole(Guid userId, AccessLevel accessLevel, Guid requestorId)
    {
        if (IsAllowedTo(_roles.GetUserRole(requestorId)!, AccessLevel.Administrator) is var result && !result.Succeeded)
        {
            return result;
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
        if (IsAllowedTo(_roles.GetUserRole(requestorId)!, AccessLevel.Administrator) is var result && !result.Succeeded)
        {
            return result;
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
        var requestorRole = _roles.GetUserRole(requestorId)!;
        if (IsAllowedTo(requestorRole, AccessLevel.Editor) is var result && !result.Succeeded)
        {
            return result;
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
        var requestorRole = _roles.GetUserRole(requestorId)!;
        if (IsAllowedTo(requestorRole, AccessLevel.Editor) is var result && !result.Succeeded)
        {
            return result;
        }

        var removed = _linkedDatasets.RemoveWhere(link => link.DatasetId == datasetId) > 0;
        if (removed)
        {
            return DomainOperationResult.Success;
        }

        return DomainOperationResult.Failure("Dataset is not linked to this project.");
    }
}