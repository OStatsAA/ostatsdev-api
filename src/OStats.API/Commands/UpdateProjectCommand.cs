using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Commands;

public class UpdateProjectCommand : CreateProjectCommand, IRequest<ICommandResult<Project>>
{
    public Guid Id { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public UpdateProjectCommand(Guid id, string userAuthId, string title, DateTime lastUpdatedAt, string? description = null) : base(userAuthId, title, description)
    {
        Id = id;
        LastUpdatedAt = lastUpdatedAt;
    }
}