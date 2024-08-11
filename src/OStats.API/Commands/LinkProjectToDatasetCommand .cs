using MediatR;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public sealed class LinkProjectToDatasetCommand : IRequest<DomainOperationResult>
{
    public string UserAuthId { get; }
    public Guid DatasetId { get; }
    public Guid ProjectId { get; }
    public LinkProjectToDatasetCommand(string userAuthId, Guid datasetId, Guid projectId)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
        ProjectId = projectId;
    }
}