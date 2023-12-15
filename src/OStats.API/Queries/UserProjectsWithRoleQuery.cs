using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Queries;

public class UserProjectsWithRoleQuery : IRequest<ICommandResult<List<UserProjectDto>>>
{
    public Guid UserId { get; }
    public string UserAuthId { get; }

    public UserProjectsWithRoleQuery(string userAuthId, Guid userId)
    {
        UserAuthId = userAuthId;
        UserId = userId;
    }
}