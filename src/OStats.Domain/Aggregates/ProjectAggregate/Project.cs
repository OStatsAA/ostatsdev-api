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

    private Project(string title, string? description)
    {
        Title = title;
        Description = description;
    }

    public Project(Guid ownerId, string title, string? description)
    {
        Title = title;
        Description = description;
        AddOrUpdateUserRole(ownerId, AccessLevel.Owner);
    }

    public void AddOrUpdateUserRole(Guid userId, AccessLevel accessLevel)
    {
        var userRole = _roles.GetUserRole(userId);

        if (userRole != null)
        {
            userRole.AccessLevel = accessLevel;
            return;
        }

        _roles.Add(new Role(Id, userId, accessLevel));
    }

    public void RemoveUserRole(Guid userId)
    {
        var userRole = _roles.GetUserRole(userId);
        if (userRole != null)
        {
            _roles.Remove(userRole);
        }
    }

    public Role? GetUserRole(Guid userId)
    {
        return _roles.Find(role => role.UserId == userId);
    }

    public bool LinkDataset(Guid datasetId)
    {
        if (_linkedDatasets.Any(link => link.DatasetId == datasetId))
        {
            return false;
        }
        return _linkedDatasets.Add(new DatasetProjectLink(datasetId, Id));
    }

    public bool UnlinkDataset(Guid datasetId)
    {
        var link = _linkedDatasets.Single(link => link.DatasetId == datasetId);
        return _linkedDatasets.Remove(link);
    }
}