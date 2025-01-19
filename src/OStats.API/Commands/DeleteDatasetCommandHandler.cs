using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class DeleteDatasetCommandHandler : CommandHandler<DeleteDatasetCommand, DomainOperationResult>
{
    public DeleteDatasetCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(DeleteDatasetCommand command, CancellationToken cancellationToken)
    {
        var dataset = await _context.Datasets.FindAsync(command.DatasetId, cancellationToken);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        dataset.Delete(command.RequestorUserId);
        await SaveCommandHandlerChangesAsync(cancellationToken);

        return DomainOperationResult.Success;
    }
}