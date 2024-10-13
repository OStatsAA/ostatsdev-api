using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class AddUserToDatasetCommandHandler : CommandHandler<AddUserToDatasetCommand, DomainOperationResult>
{
    public AddUserToDatasetCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(AddUserToDatasetCommand command, CancellationToken cancellationToken)
    {
        var dataset = await _context.Datasets.FindAsync(command.DatasetId, cancellationToken);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        var requestor = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (requestor is null)
        {
            return DomainOperationResult.Failure("Requestor not found.");
        }

        var user = await _context.Users.FindAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return DomainOperationResult.Failure("User not found.");
        }

        var result = dataset.GrantUserAccess(command.UserId, command.AccessLevel, requestor.Id);
        if (!result.Succeeded)
        {
            return result;
        }

        await SaveCommandHandlerChangesAsync(cancellationToken);
        return result;
    }
}
