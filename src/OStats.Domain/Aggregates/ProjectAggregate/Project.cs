using OStats.Domain.Common;
using OStats.Domain.Aggregates.ProjectAggregate.Events;

namespace OStats.Domain.Aggregates.ProjectAggregate;

public sealed class Project : AggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    private readonly HashSet<DatasetProjectLink> _linkedDatasets = [];
    public IReadOnlyCollection<DatasetProjectLink> LinkedDatasets => _linkedDatasets;

    public static Project Create(string title, string description, Guid requestorId, out Role requestorRole)
    {
        var project = new Project(title, description)
        {
            Id = Guid.NewGuid()
        };
        requestorRole = new Role(project.Id, requestorId, AccessLevel.Owner);
        return project;
    }

    private Project()
    {
    }

    private Project(string title, string description)
    {
        Title = title;
        Description = description;
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

        _domainEvents.Add(new TitleUpdateDomainEvent { ProjectId = Id, RequestorId = requestorRole.UserId, OldTitle = Title, Title = title });
        Title = title;
        return result;
    }

    public DomainOperationResult SetDescription(string description, Role requestorRole)
    {
        if (IsAllowedTo(requestorRole, AccessLevel.Editor) is var result && !result.Succeeded)
        {
            return result;
        }

        _domainEvents.Add(new DescriptionUpdateDomainEvent { ProjectId = Id, RequestorId = requestorRole.UserId, OldDescription = Description, Description = description });
        Description = description;
        return result;
    }

    public (DomainOperationResult, Role?) CreateUserRole(Guid userId, AccessLevel accessLevel, Role requestorRole)
    {
        if (IsAllowedTo(requestorRole, AccessLevel.Administrator) is var result && !result.Succeeded)
        {
            return (result, null);
        }

        return (DomainOperationResult.Success, new Role(Id, userId, accessLevel));
    }

    public DomainOperationResult UpdateUserRole(ref Role userRole, AccessLevel accessLevel, Role requestorRole)
    {
        if (IsAllowedTo(requestorRole, AccessLevel.Administrator) is var result && !result.Succeeded)
        {
            return result;
        }

        userRole.AccessLevel = accessLevel;
        return DomainOperationResult.Success;
    }

    public DomainOperationResult DeleteUserRole(ref Role userRoole, Role requestorRole)
    {
        if (IsAllowedTo(requestorRole, AccessLevel.Administrator) is var result && !result.Succeeded)
        {
            return result;
        }

        userRoole.IsDeleted = true;
        return DomainOperationResult.Success;
    }

    public DomainOperationResult LinkDataset(Guid datasetId, Role requestorRole)
    {
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

    public DomainOperationResult UnlinkDataset(Guid datasetId, Role requestorRole)
    {
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

    public DomainOperationResult Delete(Role requestorRole)
    {
        if (IsAllowedTo(requestorRole, AccessLevel.Owner) is var result && !result.Succeeded)
        {
            return result;
        }

        _domainEvents.Add(new DeletedProjectDomainEvent { ProjectId = Id, RequestorId = requestorRole.UserId });
        IsDeleted = true;
        return DomainOperationResult.Success;
    }
}