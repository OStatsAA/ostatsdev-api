using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class RemoveUserFromDatasetCommandHandler : CommandHandler<RemoveUserFromDatasetCommand, DomainOperationResult>
{
    public RemoveUserFromDatasetCommandHandler(Context contex, IPublishEndpoint publishEndpoint) : base(contex, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(RemoveUserFromDatasetCommand command, CancellationToken cancellationToken)
    {
        var dataset = await _context.Datasets.FindAsync(command.DatasetId, cancellationToken);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        var result = dataset.RemoveUserAccess(command.UserId, command.RequestorUserId);
        if (!result.Succeeded)
        {
            return result;
        }

        await SaveCommandHandlerChangesAsync(cancellationToken);
        return result;
    }
}