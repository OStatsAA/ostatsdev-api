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

    public override async Task<DomainOperationResult> Handle(RemoveUserFromProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FindAsync([command.ProjectId], cancellationToken);
        if (project is null)
        {
            return DomainOperationResult.Failure("Project not found.");
        }

        var result = project.RemoveUserRole(command.UserId, command.RequestorUserId);

        if (!result.Succeeded)
        {
            return result;
        }

        await SaveCommandHandlerChangesAsync(cancellationToken);

        return result;
    }
}