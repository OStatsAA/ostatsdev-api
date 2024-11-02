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

    public override async Task<DomainOperationResult> Handle(UpdateDatasetVisibilityCommand request, CancellationToken cancellationToken)
    {
        var dataset = await _context.Datasets.FindAsync(request.DatasetId, cancellationToken);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        var requestor = await _context.Users.FindByAuthIdentityAsync(request.UserAuthId, cancellationToken);
        if (requestor is null)
        {
            return DomainOperationResult.Failure("Requestor not found.");
        }

        var result = dataset.UpdateVisibility(requestor.Id, request.IsPublic);
        if (!result.Succeeded)
        {
            return result;
        }

        await SaveCommandHandlerChangesAsync(cancellationToken);
        return result;
    }
}
