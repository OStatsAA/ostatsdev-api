using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Queries;

public class ProjectUsersAndRolesQuery : IRequest<ICommandResult<List<ProjectUserAndRoleDto>>>
{
    public string UserAuthId { get; }
    public Guid ProjectId { get; }

    public ProjectUsersAndRolesQuery(string userAuthId, Guid projectId)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
    }
}