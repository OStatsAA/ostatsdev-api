using MediatR;
using OStats.API.Common;

namespace OStats.API.Commands;

public class UnlinkProjectToDatasetCommand : IRequest<ICommandResult<bool>>
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }
    public Guid ProjectId { get; }
    public UnlinkProjectToDatasetCommand(string userAuthId, Guid datasetId, Guid projectId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
        ProjectId = projectId;
    }
}