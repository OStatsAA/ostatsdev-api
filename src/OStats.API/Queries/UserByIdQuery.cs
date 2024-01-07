using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Queries;

public class UserByIdQuery : IRequest<ICommandResult<BaseUserDto>>
{
    public string UserAuthId { get; }
    public Guid UserId { get; }

    public UserByIdQuery(string userAuthId, Guid userId)
    {
        UserAuthId = userAuthId;
        UserId = userId;
    }
}