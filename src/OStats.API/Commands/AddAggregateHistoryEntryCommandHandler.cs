using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class AddAggregateHistoryEntryCommandHandler : CommandHandler<AddAggregateHistoryEntryCommand, bool>
{
    public AddAggregateHistoryEntryCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<bool> Handle(AddAggregateHistoryEntryCommand request, CancellationToken cancellationToken)
    {
        _context.AggregatesHistoryEntries.Add(new AggregateHistoryEntry(
            request.AggregateId,
            request.AggregateType,
            request.UserId,
            request.EventType,
            request.EventData,
            request.EventDescription,
            request.TimeStamp
        ));

        await SaveCommandHandlerChangesAsync(cancellationToken);
        return true;
    }
}