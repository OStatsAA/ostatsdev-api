using MassTransit;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate.Events;

namespace OStats.API.Consumers;

public sealed class DeletedProjectDomainEventConsumer : IConsumer<DeletedProjectDomainEvent>
{
    private readonly CleanupDeletedProjectResourcesCommandHandler _handler;
    public DeletedProjectDomainEventConsumer(CleanupDeletedProjectResourcesCommandHandler handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public async Task Consume(ConsumeContext<DeletedProjectDomainEvent> context)
    {
        var command = new CleanupDeletedProjectResourcesCommand { ProjectId = context.Message.ProjectId };
        var result = await _handler.Handle(command, context.CancellationToken);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(result.ErrorMessage);
        }
    }
}