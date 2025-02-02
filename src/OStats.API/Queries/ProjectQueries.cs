using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public static class ProjectQueries
{
    public static Task<Project?> GetProjectByIdAsync(Context context, Guid requestorUserId, Guid projectId, CancellationToken cancellationToken)
    {
        return GetProjectById(context, requestorUserId, projectId).WaitAsync(cancellationToken);
    }

    private static readonly Func<Context, Guid, Guid, Task<Project?>> GetProjectById = EF.CompileAsyncQuery(
        (Context context, Guid requestorUserId, Guid projectId) =>
            context.Projects
                .Join(
                    context.Roles,
                    project => project.Id,
                    roles => roles.ProjectId,
                    (project, role) => new { project, role.UserId })
                .Join(
                    context.Users,
                    projectAndUserId => projectAndUserId.UserId,
                    user => user.Id,
                    (projectAndUserId, user) => new { projectAndUserId.project, user.Id })
                .Where(join => join.project.Id == projectId && join.Id == requestorUserId)
                .Select(join => join.project)
                .AsNoTracking()
                .SingleOrDefault()
    );

    public static Task<List<Dataset>> GetProjectDatasetsAsync(Context context, Guid projectId, CancellationToken cancellationToken)
    {
        return context.DatasetsProjectsLinks
            .Join(
                context.Datasets.IgnoreAutoIncludes(),
                link => link.DatasetId,
                dataset => dataset.Id,
                (link, dataset) => new { link, dataset })
            .Where(join => join.link.ProjectId == projectId)
            .Select(join => join.dataset)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public static async Task<List<ProjectUserAndRoleDto>> GetProjectUsersAndRolesAsync(Context context, Guid userId, Guid projectId, CancellationToken cancellationToken)
    {
        if(!await context.Roles.AnyAsync( role => role.UserId == userId && role.AccessLevel != AccessLevel.NoAccess && role.ProjectId == projectId))
        {
            return [];
        }

        return await context.Projects
            .Join(
                context.Roles,
                project => project.Id,
                role => role.ProjectId,
                (project, role) => new { project, role })
            .Join(
                context.Users,
                projectAndRole => projectAndRole.role.UserId,
                user => user.Id,
                (projectAndRole, user) => new { projectAndRole.project, projectAndRole.role, user })
            .Where(join => join.project.Id == projectId)
            .Select(join => new ProjectUserAndRoleDto(join.user, join.role.AccessLevel))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}