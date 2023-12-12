using MediatR;
using OStats.API.Common;

namespace OStats.API.Commands;

public class DeleteProjectCommand : IRequest<ICommandResult<bool>>
{
    public string UserAuthId { get; }
    public Guid ProjectId { get; }

    public DeleteProjectCommand(string userAuthId, Guid projectId)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
    }
}