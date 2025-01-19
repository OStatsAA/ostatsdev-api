using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public static class UserQueries
{
    public static Task<List<BaseUserDto>> SearchUsersAsync(Context context, string searchInput, CancellationToken cancellationToken)
    {
        return context.Users
            .IgnoreAutoIncludes()
            .Where(user => EF.Functions.ILike(user.Name, $"%{searchInput}%"))
            .Select(user => new BaseUserDto(user))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public static Task<List<UserProjectDto>> GetUserProjectsAsync(Context context, Guid requestorUserId, Guid userId, CancellationToken cancellationToken)
    {
        return context.Roles
            .Join(
                context.Users,
                role => role.UserId,
                user => user.Id,
                (role, user) => new { role, user })
            .Join(
                context.Projects,
                roleAndUser => roleAndUser.role.ProjectId,
                project => project.Id,
                (roleAndUser, project) => new { project, roleAndUser })
            .Where(join => join.roleAndUser.user.Id == requestorUserId &&
                           join.roleAndUser.user.Id == userId)
            .Select(join => new UserProjectDto(join.project, join.roleAndUser.role))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public static Task<BaseUserDto?> GetUserByIdAsync(Context context, Guid userId, CancellationToken cancellationToken)
    {
        return GetUserById(context, userId).WaitAsync(cancellationToken);
    }

    private static readonly Func<Context, Guid, Task<BaseUserDto?>> GetUserById = EF.CompileAsyncQuery(
        (Context context, Guid userId) =>
            context.Users
                .Where(user => user.Id == userId)
                .Select(user => new BaseUserDto(user))
                .AsNoTracking()
                .SingleOrDefault()
    );

    public static Task<BaseUserDto?> GetUserByAuthIdAsync(Context context, string userAuthId, CancellationToken cancellationToken)
    {
        return GetUserByAuthId(context, userAuthId).WaitAsync(cancellationToken);
    }

    public static readonly Func<Context, string, Task<BaseUserDto?>> GetUserByAuthId = EF.CompileAsyncQuery(
        (Context context, string userAuthId) =>
            context.Users
                .Where(user => user.AuthIdentity == userAuthId)
                .Select(user => new BaseUserDto(user))
                .AsNoTracking()
                .SingleOrDefault()
    );

    public static Task<List<UserDatasetDto>> GetUserDatasetsAsync(Context context, Guid requestorUserId, Guid userId, CancellationToken cancellationToken)
    {
        return context.DatasetsUsersAccessLevels
            .Join(
                context.Datasets,
                userAccess => userAccess.DatasetId,
                dataset => dataset.Id,
                (userAccess, dataset) => new { dataset, userAccess })
            .Join(
                context.Users,
                datasetAndUserId => datasetAndUserId.userAccess.UserId,
                user => user.Id,
                (datasetAndUserId, user) => new { datasetAndUserId, user })
            .Where(joined => joined.datasetAndUserId.userAccess.UserId == userId &&
                                joined.user.Id == requestorUserId)
            .Select(join => new UserDatasetDto(join.datasetAndUserId.dataset, join.datasetAndUserId.userAccess))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}