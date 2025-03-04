using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public static class DatasetQueries
{
    public static Task<Dataset?> GetDatasetByIdAsync(Context context, Guid requestorUserId, Guid datasetId, CancellationToken cancellationToken)
    {
        return context.DatasetsUsersAccessLevels
            .Join(
                context.Datasets,
                userAccess => userAccess.DatasetId,
                dataset => dataset.Id,
                (userAccess, dataset) => new { dataset, userAccess.UserId })
            .Join(
                context.Users,
                datasetAndUserId => datasetAndUserId.UserId,
                user => user.Id,
                (datasetAndUserId, user) => new { datasetAndUserId, user })
            .Where(joined => joined.datasetAndUserId.dataset.Id == datasetId &&
                                joined.user.Id == requestorUserId)
            .Select(joined => joined.datasetAndUserId.dataset)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
    }

    public static Task<List<User>> GetDatasetUsersAsync(Context context, Guid datasetId, CancellationToken cancellationToken)
    {
        return context.DatasetsUsersAccessLevels
            .Where(datasetAccess => datasetAccess.DatasetId == datasetId)
            .Join(
                context.Users,
                datasetAndUserId => datasetAndUserId.UserId,
                user => user.Id,
                (_, user) => user)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public static Task<List<DatasetProjectLinkDto>> GetDatasetLinkedProjectsAsync(Context context, Guid datasetId, CancellationToken cancellationToken)
    {
        return context.DatasetsProjectsLinks
            .Join(
                context.Projects.IgnoreAutoIncludes(),
                link => link.ProjectId,
                project => project.Id,
                (link, project) => new { link, project })
            .Join(
                context.Datasets.IgnoreAutoIncludes(),
                linkProjectJoin => linkProjectJoin.link.DatasetId,
                dataset => dataset.Id,
                (linkProjectJoin, dataset) => new { linkProjectJoin.link, linkProjectJoin.project, dataset })
            .Where(join => join.link.DatasetId == datasetId)
            .Select(join => new DatasetProjectLinkDto(join.link, join.dataset, join.project))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public static Task<DatasetAccessLevel> GetUserDatasetAccessLevelAsync(Context context, Guid requestorUserId, Guid datasetId, CancellationToken cancellationToken)
    {
        return GetUserDatasetAccessLevel(context, requestorUserId, datasetId).WaitAsync(cancellationToken);
    }

    private static readonly Func<Context, Guid, Guid, Task<DatasetAccessLevel>> GetUserDatasetAccessLevel = EF.CompileAsyncQuery(
        (Context context, Guid requestorUserId, Guid datasetId) =>
            context.DatasetsUsersAccessLevels
                .Join(
                    context.Users.IgnoreAutoIncludes(),
                    access => access.UserId,
                    user => user.Id,
                    (access, user) => new { access, user })
                .AsNoTracking()
                .Where(join => join.user.Id == requestorUserId &&
                            join.access.DatasetId == datasetId)
                .Select(join => join.access.AccessLevel)
                .SingleOrDefault()
    );
}