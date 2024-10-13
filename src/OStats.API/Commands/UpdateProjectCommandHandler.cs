using MassTransit;
using OStats.API.Commands.Common;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class UpdateProjectCommandHandler : CommandHandler<UpdateProjectCommand, ValueTuple<DomainOperationResult, BaseProjectDto?>>
{
    public UpdateProjectCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<ValueTuple<DomainOperationResult, BaseProjectDto?>> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FindAsync(command.Id, cancellationToken);
        if (project is null)
        {
            return (DomainOperationResult.Failure("Project not found."), null);
        }

        if (project.LastUpdatedAt > command.LastUpdatedAt)
        {
            return (DomainOperationResult.Failure("Project has changed since command was submited."), null);
        }

        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return (DomainOperationResult.Failure("User not found."), null);
        }

        var minimumAccessLevelRequired = AccessLevel.Editor;
        if (!project.Roles.IsUserAtLeast(user.Id, minimumAccessLevelRequired))
        {
            return (DomainOperationResult.Failure("User does not have the required access level."), null);
        }

        project.Title = command.Title;
        project.Description = command.Description;

        await SaveCommandHandlerChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseProjectDto(project));
    }
}