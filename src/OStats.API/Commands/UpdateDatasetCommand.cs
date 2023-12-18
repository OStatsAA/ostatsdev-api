using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.API.Commands;

public class UpdateDatasetCommand : CreateDatasetCommand, IRequest<ICommandResult<Dataset>>
{
    public Guid Id { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public UpdateDatasetCommand(Guid id, string userAuthId, string title, string source, DateTime lastUpdatedAt, string? description = null) : base(userAuthId, title, source, description)
    {
        Id = id;
        LastUpdatedAt = lastUpdatedAt;
    }
}