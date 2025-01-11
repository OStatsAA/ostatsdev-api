using MassTransit;
using OStats.API.Commands;
using OStats.Domain.Aggregates.UserAggregate.Events;

namespace OStats.API.Consumers;

public sealed class DeletedUserDomainEventConsumer : IConsumer<DeletedUserDomainEvent>
{
    private readonly CleanupDeletedUserResourcesCommandHandler _handler;
    public DeletedUserDomainEventConsumer(CleanupDeletedUserResourcesCommandHandler handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public async Task Consume(ConsumeContext<DeletedUserDomainEvent> context)
    {
        var command = new CleanupDeletedUserResourcesCommand { UserId = context.Message.UserId };
        var result = await _handler.Handle(command, context.CancellationToken);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(result.ErrorMessage);
        }
    }
}