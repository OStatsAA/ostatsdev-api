using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ValueTuple<DomainOperationResult, BaseProjectDto?>>
{
    private readonly Context _context;

    public UpdateProjectCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ValueTuple<DomainOperationResult, BaseProjectDto?>> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
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

        await _context.SaveChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseProjectDto(project));
    }
}