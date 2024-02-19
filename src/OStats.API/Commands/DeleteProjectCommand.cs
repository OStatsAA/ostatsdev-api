using MediatR;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public class DeleteProjectCommand : IRequest<DomainOperationResult>
{
    public string UserAuthId { get; }
    public Guid ProjectId { get; }

    public DeleteProjectCommand(string userAuthId, Guid projectId)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
    }
}