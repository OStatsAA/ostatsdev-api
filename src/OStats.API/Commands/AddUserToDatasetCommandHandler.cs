using MassTransit;
using MediatR;
using OStats.API.Extensions;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class AddUserToDatasetCommandHandler : IRequestHandler<AddUserToDatasetCommand, DomainOperationResult>
{
    private readonly Context _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public AddUserToDatasetCommandHandler(Context context, IPublishEndpoint publishEndpoint)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
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

        await _publishEndpoint.PublishDomainEventsAsync(dataset, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return result;
    }
}
