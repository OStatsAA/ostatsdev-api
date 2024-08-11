using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public sealed class UpdateProjectCommand : CreateProjectCommand, IRequest<ValueTuple<DomainOperationResult, BaseProjectDto?>>
{
    public Guid Id { get; init; }
    public DateTime LastUpdatedAt { get; init; }

    public UpdateProjectCommand(Guid id, string userAuthId, string title, DateTime lastUpdatedAt, string? description) : base(userAuthId, title, description)
    {
        Id = id;
        LastUpdatedAt = lastUpdatedAt;
    }
}