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

    public override async Task<DomainOperationResult> Handle(AddUserToProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FindAsync(command.ProjectId, cancellationToken);
        if (project is null)
        {
            return DomainOperationResult.Failure("Project not found");
        }

        var roles = await _context.Roles.FindByProjectIdAndUsersIdsAsync(command.ProjectId, [command.RequestorId, command.UserId], cancellationToken);
        if (roles.TryGetValue(command.RequestorId, out var requestorRole) is false)
        {
            return DomainOperationResult.InvalidUserRole();
        }

        if (roles.TryGetValue(command.UserId, out var userRole) is true && userRole.AccessLevel == command.AccessLevel)
        {
            return DomainOperationResult.NoActionTaken($"User is already in the project with {userRole.AccessLevel} level.");
        }

        DomainOperationResult result;
        if (userRole is not null)
        {
            result = project.UpdateUserRole(ref userRole, command.AccessLevel, requestorRole);
        }
        else
        {
            (result, userRole) = project.CreateUserRole(command.UserId, command.AccessLevel, requestorRole);
        }

        if (!result.Succeeded || userRole is null)
        {
            return result;
        }

        await _context.AddAsync(userRole, cancellationToken);
        await SaveCommandHandlerChangesAsync(cancellationToken);
        return result;
    }
}
