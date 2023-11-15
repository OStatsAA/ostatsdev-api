using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.ProjectAggregate;

public class Project : Entity, IAggregateRoot
{
    public string Title { get; }
    public string? Description { get; }
    private readonly List<Role> _roles = new List<Role>();
    public IReadOnlyCollection<Role> Roles => _roles;


    public Project(Guid ownerId, string title, string? description)
    {
        Title = title;
        Description = description;
        AddOrUpdateUserRole(ownerId, AccessLevel.Owner);
    }

    public void AddOrUpdateUserRole(Guid userId, AccessLevel accessLevel)
    {
        var userRole = _roles.SingleOrDefault(role => role.UserId == userId);

        if (userRole != null)
        {
            userRole.AccessLevel = accessLevel;
            return;
        }

        _roles.Add(new Role(Id, userId, accessLevel));
    }

    public void RemoveUserRole(Guid userId)
    {
        var userRole = _roles.SingleOrDefault(role => role.UserId == userId);
        if (userRole != null)
        {
            _roles.Remove(userRole);
        }
    }

    public Role? GetUserRole(Guid userId)
    {
        return _roles.Find(role => role.UserId == userId);
    }
}