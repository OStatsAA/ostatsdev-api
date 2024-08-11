using MassTransit;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class AddAggregateHistoryEntryCommandHandler : IConsumer<AddAggregateHistoryEntryCommand>
{
    private readonly Context _context;

    public AddAggregateHistoryEntryCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task Consume(ConsumeContext<AddAggregateHistoryEntryCommand> context)
    {
        var request = context.Message;
        _context.AggregatesHistoryEntries.Add(new AggregateHistoryEntry(
            request.AggregateId,
            request.AggregateType,
            request.UserId,
            request.EventType,
            request.EventData,
            request.EventDescription,
            request.TimeStamp
        ));
        await _context.SaveChangesAsync();
    }

}