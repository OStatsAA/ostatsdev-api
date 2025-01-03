using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class DeleteProjectCommandHandler : CommandHandler<DeleteProjectCommand, DomainOperationResult>
{
    public DeleteProjectCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return DomainOperationResult.Failure("User not found.");
        }

        var project = await _context.Projects.FindAsync(command.ProjectId, cancellationToken);
        if (project is null)
        {
            return DomainOperationResult.Failure("Project not found.");
        }

        var isUserOwner = project.Roles.IsUser(user.Id, AccessLevel.Owner);
        if (!isUserOwner)
        {
            return DomainOperationResult.Failure("User does not have permission to delete this project.");
        }

        _context.Remove(project);
        await SaveCommandHandlerChangesAsync(cancellationToken);

        return DomainOperationResult.Success;
    }
}