using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Queries;

public class ProjectByIdQuery : IRequest<ICommandResult<ProjectDto>>
{
    public string UserAuthId { get; }
    public Guid ProjectId { get; }

    public ProjectByIdQuery(string userAuthId, Guid projectId)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
    }
}