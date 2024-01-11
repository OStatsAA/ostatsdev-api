using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.API.Commands;

public class AddUserToDatasetCommand : IRequest<ICommandResult<bool>>
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