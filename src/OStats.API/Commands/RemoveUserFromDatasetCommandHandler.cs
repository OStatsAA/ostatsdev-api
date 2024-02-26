using MediatR;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class RemoveUserFromDatasetCommandHandler : IRequestHandler<RemoveUserFromDatasetCommand, DomainOperationResult>
{
    private readonly Context _context;

    public RemoveUserFromDatasetCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DomainOperationResult> Handle(RemoveUserFromDatasetCommand request, CancellationToken cancellationToken)
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

        var result = dataset.RemoveUserAccess(request.UserId, requestor.Id);
        if (!result.Succeeded)
        {
            return result;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return result;
    }
}