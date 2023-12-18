using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.API.Queries;

public class DatasetByIdQuery : IRequest<ICommandResult<Dataset>>
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }

    public DatasetByIdQuery(string userAuthId, Guid datasetId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
    }
}