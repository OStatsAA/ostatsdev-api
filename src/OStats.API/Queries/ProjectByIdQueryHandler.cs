using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class ProjectByIdQueryHandler : IRequestHandler<ProjectByIdQuery, ICommandResult<ProjectDto>>
{
    private readonly Context _context;

    public ProjectByIdQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<ProjectDto>> Handle(ProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .Join(
                _context.Roles,
                project => project.Id,
                roles => roles.ProjectId,
                (project, role) => new { project, role.UserId })
            .Join(
                _context.Users,
                projectAndUserId => projectAndUserId.UserId,
                user => user.Id,
                (projectAndUserId, user) => new { projectAndUserId.project, user.AuthIdentity })
            .Where(join => join.project.Id == request.ProjectId && join.AuthIdentity == request.UserAuthId)
            .Select(join => join.project)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (project is null)
        {
            var error = new ValidationFailure("ProjectId", "Project not found.");
            return new CommandResult<ProjectDto>(error);
        }

        var linkedDatasets = await _context.DatasetsProjectsLinks
            .Join(
                _context.Datasets.IgnoreAutoIncludes(),
                link => link.DatasetId,
                dataset => dataset.Id,
                (link, dataset) => new { link, dataset })
            .Where(join => join.link.ProjectId == project.Id)
            .Select(join => join.dataset)
            .ToListAsync(cancellationToken);

        var projectDto = new ProjectDto(project, linkedDatasets);

        return new CommandResult<ProjectDto>(projectDto);
    }
}