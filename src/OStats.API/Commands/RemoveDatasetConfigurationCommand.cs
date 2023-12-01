using MediatR;
using OStats.API.Common;

namespace OStats.API.Commands;

public class RemoveDatasetConfigurationCommand : IRequest<ICommandResult<bool>>
{
    public string UserAuthId { get; }
    public Guid ProjectId { get; }
    public Guid DatasetConfigurationId { get; }

    public RemoveDatasetConfigurationCommand(string userAuthId, Guid projectId, Guid datasetConfigId)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
        DatasetConfigurationId = datasetConfigId;
    }
}
