using System.Text.Json;
using MassTransit;
using OStats.API.Commands;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API.Consumers;

public class GrantedUserAccessToDatasetDomainEventConsumer : IConsumer<GrantedUserAccessToDatasetDomainEvent>
{
    private readonly Context _context;
    private readonly AddAggregateHistoryEntryCommandHandler _handler;

    public GrantedUserAccessToDatasetDomainEventConsumer(Context context, AddAggregateHistoryEntryCommandHandler handler)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public async Task Consume(ConsumeContext<GrantedUserAccessToDatasetDomainEvent> consumeContext)
    {
        var domainEvent = consumeContext.Message;
        var sentTime = consumeContext.SentTime ?? DateTime.UtcNow;
        var dataset = await _context.Datasets.FindAsync(domainEvent.DatasetUserAccessLevel.DatasetId);
        var requestor = await _context.Users.FindAsync(domainEvent.RequestorId);
        var user = await _context.Users.FindAsync(domainEvent.DatasetUserAccessLevel.UserId);

        var eventDescription = domainEvent.GetEventDescription(
            dataset?.Title ?? domainEvent.DatasetUserAccessLevel.DatasetId.ToString(),
            requestor?.Name ?? domainEvent.RequestorId.ToString(),
            user?.Name ?? domainEvent.DatasetUserAccessLevel.UserId.ToString()
        );

        await _handler.Handle(new AddAggregateHistoryEntryCommand
        {
            AggregateId = dataset?.Id ?? domainEvent.DatasetUserAccessLevel.DatasetId,
            AggregateType = nameof(Dataset),
            UserId = domainEvent.RequestorId,
            EventDescription = eventDescription,
            EventData = JsonSerializer.Serialize(domainEvent),
            EventType = nameof(GrantedUserAccessToDatasetDomainEvent),
            TimeStamp = sentTime
        }, default);
    }
}
