using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class AddUserToProjectCommandHandler : CommandHandler<AddUserToProjectCommand, DomainOperationResult>
{
    public AddUserToProjectCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(AddUserToProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FindAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return DomainOperationResult.Failure("Project not found");
        }

        var requestor = await _context.Users.FindByAuthIdentityAsync(request.UserAuthId, cancellationToken);
        if (requestor is null)
        {
            return DomainOperationResult.Failure("Requestor not found");
        }

        var result = project.AddOrUpdateUserRole(request.UserId, request.AccessLevel, requestor.Id);
        if (!result.Succeeded)
        {
            return result;
        }

        await SaveCommandHandlerChangesAsync(cancellationToken);
        return result;
    }
}
