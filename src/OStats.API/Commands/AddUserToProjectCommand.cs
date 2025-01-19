using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Commands;

public sealed record AddUserToProjectCommand
{
    public Guid RequestorUserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public AccessLevel AccessLevel { get; set; }

    public AddUserToProjectCommand(Guid requestorUserId, Guid projectId, Guid userId, AccessLevel accessLevel)
    {
        RequestorUserId = requestorUserId;
        UserId = userId;
        ProjectId = projectId;
        AccessLevel = accessLevel;
    }
}