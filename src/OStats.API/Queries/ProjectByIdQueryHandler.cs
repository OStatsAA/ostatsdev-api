using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Queries;

public class ProjectByIdQueryHandler : IRequestHandler<ProjectByIdQuery, ICommandResult<Project>>
{
    private readonly Context _context;

    public ProjectByIdQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<Project>> Handle(ProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindByAuthIdentityAsync(request.UserAuthId, cancellationToken);
        if (user is null)
        {
            var error = new ValidationFailure("UserAuthId", "User not found.");
            return new CommandResult<Project>(error);
        }

        var project = await _context.Roles
            .AsNoTracking()
            .Where(role => role.ProjectId == request.ProjectId && role.UserId == user.Id)
            .Join(_context.Projects, role => role.ProjectId, project => project.Id, (role, project) => project)
            .SingleOrDefaultAsync(cancellationToken);

        if (project is null)
        {
            var error = new ValidationFailure("ProjectId", "Project not found.");
            return new CommandResult<Project>(error);
        }

        return new CommandResult<Project>(project);
    }
}