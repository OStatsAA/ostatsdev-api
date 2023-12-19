using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class UserProjectsWithRoleQueryHandler : IRequestHandler<UserProjectsWithRoleQuery, ICommandResult<List<UserProjectDto>>>
{
    private readonly Context _context;

    public UserProjectsWithRoleQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<List<UserProjectDto>>> Handle(UserProjectsWithRoleQuery request, CancellationToken cancellationToken)
    {
        var userProjects = await _context.Roles
            .Join(
                _context.Users,
                role => role.UserId,
                user => user.Id,
                (role, user) => new { role, user })
            .Join(
                _context.Projects,
                roleAndUser => roleAndUser.role.ProjectId,
                project => project.Id,
                (roleAndUser, project) => new { project, roleAndUser })
            .Where(join => join.roleAndUser.user.AuthIdentity == request.UserAuthId && join.roleAndUser.user.Id == request.UserId)
            .Select(join => new UserProjectDto(join.project, join.roleAndUser.role))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new CommandResult<List<UserProjectDto>>(userProjects);
    }
}