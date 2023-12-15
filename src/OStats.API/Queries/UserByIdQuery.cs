using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Queries;

public class UserByIdQuery : IRequest<ICommandResult<User>>
{
    public string UserAuthId { get; }
    public Guid UserId { get; }

    public UserByIdQuery(string userAuthId, Guid userId)
    {
        UserAuthId = userAuthId;
        UserId = userId;
    }
}