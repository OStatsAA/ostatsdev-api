using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public class UpdateDatasetCommand : CreateDatasetCommand, IRequest<ValueTuple<DomainOperationResult, BaseDatasetDto?>>
{
    public Guid Id { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public UpdateDatasetCommand(Guid id, string userAuthId, string title, string source, DateTime lastUpdatedAt, string? description = null) : base(userAuthId, title, source, description)
    {
        Id = id;
        LastUpdatedAt = lastUpdatedAt;
    }
}