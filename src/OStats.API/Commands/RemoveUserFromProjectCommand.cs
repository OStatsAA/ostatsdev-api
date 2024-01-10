using MediatR;
using OStats.API.Common;

namespace OStats.API.Commands;

public class RemoveUserFromProjectCommand : IRequest<ICommandResult<bool>>
{
    public string UserAuthId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    public RemoveUserFromProjectCommand(string userAuthId, Guid projectId, Guid userId)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
        UserId = userId;
    }
}