using MediatR;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public sealed class AddUserToProjectCommand : IRequest<DomainOperationResult>
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