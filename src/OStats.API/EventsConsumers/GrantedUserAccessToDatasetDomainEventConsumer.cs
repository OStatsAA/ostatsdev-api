using System.Text.Json;
using MassTransit;
using OStats.API.Commands;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API.DomainEventsHandlers;

public class GrantedUserAccessToDatasetDomainEventConsumer : IConsumer<GrantedUserAccessToDatasetDomainEvent>
{
    private readonly Context _context;

    public GrantedUserAccessToDatasetDomainEventConsumer(Context context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<GrantedUserAccessToDatasetDomainEvent> context)
    {
        var domainEvent = context.Message;
        var sentTime = context.SentTime ?? DateTime.UtcNow;
        var dataset = await _context.Datasets.FindAsync(domainEvent.DatasetUserAccessLevel.DatasetId);
        var requestor = await _context.Users.FindAsync(domainEvent.RequestorId);
        var user = await _context.Users.FindAsync(domainEvent.DatasetUserAccessLevel.UserId);

        var eventDescription = domainEvent.GetEventDescription(
            dataset?.Title ?? domainEvent.DatasetUserAccessLevel.DatasetId.ToString(),
            requestor?.Name ?? domainEvent.RequestorId.ToString(),
            user?.Name ?? domainEvent.DatasetUserAccessLevel.UserId.ToString()
        );

        await context.Publish(new AddAggregateHistoryEntryCommand
        {
            AggregateId = dataset?.Id ?? domainEvent.DatasetUserAccessLevel.DatasetId,
            AggregateType = nameof(Dataset),
            UserId = domainEvent.RequestorId,
            EventDescription = eventDescription,
            EventData = JsonSerializer.Serialize(domainEvent),
            EventType = nameof(GrantedUserAccessToDatasetDomainEvent),
            TimeStamp = sentTime
        });
    }
}
