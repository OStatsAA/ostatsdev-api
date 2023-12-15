using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Queries;

public class UserByAuthIdQuery : IRequest<ICommandResult<User>>
{
    public string UserAuthId { get; }

    public UserByAuthIdQuery(string userAuthId)
    {
        UserAuthId = userAuthId;
    }
}