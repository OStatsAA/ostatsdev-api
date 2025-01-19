using MassTransit;
using OStats.API.Commands.Common;
using OStats.API.Dtos;
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
        var project = await _context.Projects.FindAsync([command.Id], cancellationToken);
        if (project is null)
        {
            return (DomainOperationResult.Failure("Project not found."), null);
        }

        if (project.LastUpdatedAt > command.LastUpdatedAt)
        {
            return (DomainOperationResult.Failure("Project has changed since command was submited."), null);
        }

        var user = await _context.Users.FindAsync([command.RequestorUserId], cancellationToken);
        if (user is null)
        {
            return (DomainOperationResult.Failure("User not found."), null);
        }

        var userRole = project.Roles.GetUserRole(user.Id);
        if (userRole is null)
        {
            return (DomainOperationResult.InvalidUserRole("User does not have a role in this project."), null);
        }

        project.SetTitle(command.Title, userRole);
        project.SetDescription(command.Description, userRole);

        await SaveCommandHandlerChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseProjectDto(project));
    }
}