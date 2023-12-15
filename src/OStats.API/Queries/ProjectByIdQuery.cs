using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Queries;

public class ProjectByIdQuery : IRequest<ICommandResult<Project>>
{
    public string UserAuthId { get; }
    public Guid ProjectId { get; }

    public ProjectByIdQuery(string userAuthId, Guid projectId)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
    }
}