using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

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
        var user = await _context.Users.FindByAuthIdentityAsync(request.UserAuthId, cancellationToken);
        if (user is null)
        {
            var error = new ValidationFailure("UserAuthId", "User not found.");
            return new CommandResult<List<UserProjectDto>>(error);
        }

        if (user.Id != request.UserId)
        {
            var error = new ValidationFailure("UserId", "User is not requestor.");
            return new CommandResult<List<UserProjectDto>>(error);
        }

        var userProjects = await _context.Roles
            .AsNoTracking()
            .Where(role => role.UserId == user.Id)
            .Join(_context.Projects,
                role => role.ProjectId,
                project => project.Id,
                (role, project) => new UserProjectDto(project, role))
            .ToListAsync(cancellationToken);

        return new CommandResult<List<UserProjectDto>>(userProjects);
    }
}