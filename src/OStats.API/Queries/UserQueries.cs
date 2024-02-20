using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public static class UserQueries
{
    public static Task<List<BaseUserDto>> SearchUsersAsync(Context context, string searchInput)
    {
        return context.Users
            .IgnoreAutoIncludes()
            .Where(user => EF.Functions.ILike(user.Name, $"%{searchInput}%"))
            .Select(user => new BaseUserDto(user))
            .AsNoTracking()
            .ToListAsync();
    }

    public static Task<List<UserProjectDto>> GetUserProjectsAsync(Context context, string userAuthId, Guid userId)
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
            .Where(join => join.roleAndUser.user.AuthIdentity == userAuthId &&
                           join.roleAndUser.user.Id == userId)
            .Select(join => new UserProjectDto(join.project, join.roleAndUser.role))
            .AsNoTracking()
            .ToListAsync();
    }

    public static readonly Func<Context, Guid, Task<BaseUserDto?>> GetUserByIdAsync = EF.CompileAsyncQuery(
        (Context context, Guid userId) =>
            context.Users
                .Where(user => user.Id == userId)
                .Select(user => new BaseUserDto(user))
                .AsNoTracking()
                .SingleOrDefault()
    );

    public static readonly Func<Context, string, Task<BaseUserDto?>> GetUserByAuthIdAsync = EF.CompileAsyncQuery(
        (Context context, string userAuthId) =>
            context.Users
                .Where(user => user.AuthIdentity == userAuthId)
                .Select(user => new BaseUserDto(user))
                .AsNoTracking()
                .SingleOrDefault()
    );

    public static Task<List<UserDatasetDto>> GetUserDatasetsAsync(Context context, string userAuthId, Guid userId)
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
                                joined.user.AuthIdentity == userAuthId)
            .Select(join => new UserDatasetDto(join.datasetAndUserId.dataset, join.datasetAndUserId.userAccess))
            .AsNoTracking()
            .ToListAsync();
    }
}