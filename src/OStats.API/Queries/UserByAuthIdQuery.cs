using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Queries;

public class UserByAuthIdQuery : IRequest<ICommandResult<BaseUserDto>>
{
    public string UserAuthId { get; }

    public UserByAuthIdQuery(string userAuthId)
    {
        UserAuthId = userAuthId;
    }
}