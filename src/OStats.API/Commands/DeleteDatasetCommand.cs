using MediatR;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public sealed class DeleteDatasetCommand : IRequest<DomainOperationResult>
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }

    public DeleteDatasetCommand(string userAuthId, Guid datasetId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
    }
}