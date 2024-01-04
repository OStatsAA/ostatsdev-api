using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Queries;

public class DatasetByIdQuery : IRequest<ICommandResult<DatasetWithUsersDto>>
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }

    public DatasetByIdQuery(string userAuthId, Guid datasetId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
    }
}