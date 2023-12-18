using MediatR;
using OStats.API.Common;

namespace OStats.API.Commands;

public class DeleteDatasetCommand : IRequest<ICommandResult<bool>>
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }

    public DeleteDatasetCommand(string userAuthId, Guid datasetId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
    }
}