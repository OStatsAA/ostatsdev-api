using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Commands;

public sealed record AddUserToProjectCommand
{
    public Guid RequestorId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public AccessLevel AccessLevel { get; set; }

    public AddUserToProjectCommand(Guid requestorId, Guid projectId, Guid userId, AccessLevel accessLevel)
    {
        RequestorId = requestorId;
        UserId = userId;
        ProjectId = projectId;
        AccessLevel = accessLevel;
    }
}