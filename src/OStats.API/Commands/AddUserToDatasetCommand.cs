using MediatR;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public sealed class AddUserToDatasetCommand : IRequest<DomainOperationResult>
{
    public string UserAuthId { get; set; }
    public Guid DatasetId { get; set; }
    public Guid UserId { get; set; }
    public DatasetAccessLevel AccessLevel { get; set; }

    public AddUserToDatasetCommand(string userAuthId, Guid datasetId, Guid userId, DatasetAccessLevel accessLevel)
    {
        UserAuthId = userAuthId;
        UserId = userId;
        DatasetId = datasetId;
        AccessLevel = accessLevel;
    }
}