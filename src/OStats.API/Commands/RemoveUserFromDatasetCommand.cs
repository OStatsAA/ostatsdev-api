using MediatR;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public sealed class RemoveUserFromDatasetCommand : IRequest<DomainOperationResult>
{
    public string UserAuthId { get; set; }
    public Guid DatasetId { get; set; }
    public Guid UserId { get; set; }

    public RemoveUserFromDatasetCommand(string userAuthId, Guid datasetId, Guid userId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
        UserId = userId;
    }
}