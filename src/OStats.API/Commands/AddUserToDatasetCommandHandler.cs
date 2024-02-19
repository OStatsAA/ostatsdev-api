using MediatR;
using OStats.Domain.Common;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Commands;

public class AddUserToDatasetCommandHandler : IRequestHandler<AddUserToDatasetCommand, DomainOperationResult>
{
    private readonly Context _context;

    public AddUserToDatasetCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<DomainOperationResult> Handle(AddUserToDatasetCommand request, CancellationToken cancellationToken)
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

        var user = await _context.Users.FindAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return DomainOperationResult.Failure("User not found.");
        }

        var result = dataset.GrantUserAccess(request.UserId, request.AccessLevel, requestor.Id);
        if (!result.Succeeded)
        {
            return result;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return result;
    }
}
