using MediatR;

namespace OStats.API.Commands;

public sealed record AddAggregateHistoryEntryCommand : IRequest<bool>
{
    public required Guid AggregateId { get; init; }
    public required string AggregateType { get; init; }
    public required Guid UserId { get; init; }
    public required string EventType { get; init; }
    public required string EventData { get; init; }
    public required string EventDescription { get; init; }
    public required DateTime TimeStamp { get; init; }
}
