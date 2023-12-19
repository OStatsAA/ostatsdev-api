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

        var projectUsersAndRoles = await _context.Projects
            .Join(
                _context.Roles,
                project => project.Id,
                role => role.ProjectId,
                (project, role) => new { project, role })
            .Join(
                _context.Users,
                projectAndRole => projectAndRole.role.UserId,
                user => user.Id,
                (projectAndRole, user) => new { projectAndRole.project, projectAndRole.role, user })
            .Where(join => join.project.Id == request.ProjectId &&
                           join.project.Roles.Any(role => role.UserId == user.Id))
            .Select(join => new ProjectUserAndRoleDto(join.user, join.role))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new CommandResult<List<ProjectUserAndRoleDto>>(projectUsersAndRoles);
    }
}