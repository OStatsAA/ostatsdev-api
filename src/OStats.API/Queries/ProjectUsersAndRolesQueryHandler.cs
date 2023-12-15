using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Queries;

public class ProjectUsersAndRolesQueryHandler : IRequestHandler<ProjectUsersAndRolesQuery, ICommandResult<List<ProjectUserAndRoleDto>>>
{
    private readonly Context _context;

    public ProjectUsersAndRolesQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<List<ProjectUserAndRoleDto>>> Handle(ProjectUsersAndRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindByAuthIdentityAsync(request.UserAuthId, cancellationToken);
        if (user is null)
        {
            var error = new ValidationFailure("UserAuthId", "User not found.");
            return new CommandResult<List<ProjectUserAndRoleDto>>(error);
        }

        var projectUsersAndRoles = await _context.Roles
            .AsNoTracking()
            .Where(role => role.ProjectId == request.ProjectId)
            .Join(_context.Users,
                role => role.UserId,
                user => user.Id,
                (role, user) => new ProjectUserAndRoleDto(user, role))
            .ToListAsync(cancellationToken);

        if (!projectUsersAndRoles.Any(ur => ur.User.Id == user.Id))
        {
            var error = new ValidationFailure("UserAuthId", "Unauthorized.");
            return new CommandResult<List<ProjectUserAndRoleDto>>(error);
        }

        return new CommandResult<List<ProjectUserAndRoleDto>>(projectUsersAndRoles);
    }
}