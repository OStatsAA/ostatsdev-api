using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Queries;

public class UserDatasetsWithAccessQuery : IRequest<ICommandResult<List<UserDatasetDto>>>
{
    public Guid UserId { get; }
    public string UserAuthId { get; }

    public UserDatasetsWithAccessQuery(string userAuthId, Guid userId)
    {
        UserAuthId = userAuthId;
        UserId = userId;
    }
}