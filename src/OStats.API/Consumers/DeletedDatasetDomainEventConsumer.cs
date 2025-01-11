using MassTransit;
using OStats.API.Commands;
using OStats.Domain.Aggregates.DatasetAggregate.Events;

namespace OStats.API.Consumers;

public sealed class DeletedDatasetDomainEventConsumer : IConsumer<DeletedDatasetDomainEvent>
{
    private readonly CleanupDeletedDatasetResourcesCommandHandler _handler;
    public DeletedDatasetDomainEventConsumer(CleanupDeletedDatasetResourcesCommandHandler handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public async Task Consume(ConsumeContext<DeletedDatasetDomainEvent> context)
    {
        var command = new CleanupDeletedDatasetResourcesCommand { DatasetId = context.Message.DatasetId };
        var result = await _handler.Handle(command, context.CancellationToken);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(result.ErrorMessage);
        }
    }
}