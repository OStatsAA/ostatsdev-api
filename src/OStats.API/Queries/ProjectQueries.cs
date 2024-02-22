using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public static class ProjectQueries
{
    public static readonly Func<Context, string, Guid, Task<Project?>> GetProjectByIdAsync = EF.CompileAsyncQuery(
        (Context context, string userAuthId, Guid projectId) =>
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
                    (projectAndUserId, user) => new { projectAndUserId.project, user.AuthIdentity })
                .Where(join => join.project.Id == projectId && join.AuthIdentity == userAuthId)
                .Select(join => join.project)
                .AsNoTracking()
                .SingleOrDefault()
    );

    public static Task<List<Dataset>> GetProjectDatasetsAsync(Context context, Guid projectId)
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
            .ToListAsync();
    }

    public static Task<List<ProjectUserAndRoleDto>> GetProjectUsersAndRolesAsync(Context context, Guid userId, Guid projectId)
    {
        return context.Projects
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
            .Where(join => join.project.Id == projectId &&
                           join.project.Roles.Any(role => role.UserId == userId))
            .Select(join => new ProjectUserAndRoleDto(join.user, join.role))
            .AsNoTracking()
            .ToListAsync();
    }
}