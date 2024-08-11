using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.ProjectAggregate;

public sealed class Role : Entity
{
    public Guid ProjectId { get; }
    public Guid UserId { get; }
    public AccessLevel AccessLevel { get; set; }

    public Role(Guid projectId, Guid userId, AccessLevel accessLevel)
    {
        ProjectId = projectId;
        UserId = userId;
        AccessLevel = accessLevel;
    }

}