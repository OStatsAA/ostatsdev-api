using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class UpdateDatasetVisibilityCommandHandler : CommandHandler<UpdateDatasetVisibilityCommand, DomainOperationResult>
{
    public UpdateDatasetVisibilityCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(UpdateDatasetVisibilityCommand command, CancellationToken cancellationToken)
    {
        var dataset = await _context.Datasets.FindAsync([command.DatasetId], cancellationToken);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        var result = dataset.UpdateVisibility(command.RequestorUserId, command.IsPublic);
        if (!result.Succeeded)
        {
            return result;
        }

        await SaveCommandHandlerChangesAsync(cancellationToken);
        return result;
    }
}
