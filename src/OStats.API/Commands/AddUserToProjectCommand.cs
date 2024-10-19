using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Commands;

public sealed record AddUserToProjectCommand
{
    public string UserAuthId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public AccessLevel AccessLevel { get; set; }

    public AddUserToProjectCommand(string userAuthId, Guid projectId, Guid userId, AccessLevel accessLevel)
    {
        UserAuthId = userAuthId;
        UserId = userId;
        ProjectId = projectId;
        AccessLevel = accessLevel;
    }
}