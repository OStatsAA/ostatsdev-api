using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class RemoveUserFromProjectCommandHandler : CommandHandler<RemoveUserFromProjectCommand, DomainOperationResult>
{
    public RemoveUserFromProjectCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(RemoveUserFromProjectCommand request, CancellationToken cancellationToken)
    {

        var requestor = await _context.Users.FindByAuthIdentityAsync(request.UserAuthId, cancellationToken);
        if (requestor is null)
        {
            return DomainOperationResult.Failure("Requestor not found.");
        }

        var project = await _context.Projects.FindAsync(request.ProjectId);
        if (project is null)
        {
            return DomainOperationResult.Failure("Project not found.");
        }

        var result = project.RemoveUserRole(request.UserId, requestor.Id);

        if (!result.Succeeded)
        {
            return result;
        }

        await SaveCommandHandlerChangesAsync(cancellationToken);

        return result;
    }
}