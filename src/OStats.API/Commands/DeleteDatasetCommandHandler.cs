using MediatR;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class DeleteDatasetCommandHandler : IRequestHandler<DeleteDatasetCommand, DomainOperationResult>
{
    private readonly Context _context;

    public DeleteDatasetCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DomainOperationResult> Handle(DeleteDatasetCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return DomainOperationResult.Failure("User not found.");
        }

        var dataset = await _context.Datasets.FindAsync(command.DatasetId, cancellationToken);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        if (dataset.GetUserAccessLevel(user.Id) < DatasetAccessLevel.Owner)
        {
            return DomainOperationResult.Failure("User does not have permission to delete this dataset.");
        }

        _context.Remove(dataset);
        await _context.SaveChangesAsync(cancellationToken);

        return DomainOperationResult.Success;
    }
}