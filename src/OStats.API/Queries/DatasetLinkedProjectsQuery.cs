using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Queries;

public class DatasetLinkedProjectsQuery : IRequest<ICommandResult<List<DatasetProjectLinkDto>>>
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }

    public DatasetLinkedProjectsQuery(string userAuthId, Guid datasetId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
    }
}