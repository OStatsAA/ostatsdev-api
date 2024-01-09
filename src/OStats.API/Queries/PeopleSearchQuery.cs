using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Queries;

public class PeopleSearchQuery : IRequest<ICommandResult<List<BaseUserDto>>>
{
    public string UserAuthId { get; }
    public string SearchInput { get; }

    public PeopleSearchQuery(string userAuthId, string searchInput)
    {
        UserAuthId = userAuthId;
        SearchInput = searchInput;
    }
}