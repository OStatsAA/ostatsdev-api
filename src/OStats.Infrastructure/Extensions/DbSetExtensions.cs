using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Infrastructure;

public static class DbSetExtensions
{
    public static Task<User?> FindByAuthIdentityAsync(this DbSet<User> users, string authIdentity, CancellationToken cancellationToken)
    {
        return users.SingleOrDefaultAsync(user => user.AuthIdentity == authIdentity, cancellationToken);
    }

    public static Task<bool> AnyByAuthIdentityAsync(this DbSet<User> users, string authIdentity, CancellationToken cancellationToken)
    {
        return users.AnyAsync(user => user.AuthIdentity == authIdentity, cancellationToken);
    }

    public static Task<bool> AnyByUserAndDatasetIdAsync(this DbSet<DatasetUserAccessLevel> datasetsUsersAccessLevels, Guid userId, Guid datasetId, CancellationToken cancellationToken)
    {
        return datasetsUsersAccessLevels.AnyAsync(accesses => accesses.UserId == userId && accesses.DatasetId == datasetId, cancellationToken);
    }

    public static Task<Role?> FindByProjectIdAndUserIdAsync(this DbSet<Role> roles, Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        return roles.SingleOrDefaultAsync(role => role.ProjectId == projectId && role.UserId == userId, cancellationToken);
    }

    public static Task<Dictionary<Guid, Role>> FindByProjectIdAndUsersIdsAsync(this DbSet<Role> roles, Guid projectId, List<Guid> usersIds, CancellationToken cancellationToken)
    {
        return roles.Where(role => role.ProjectId == projectId && usersIds.Contains(role.UserId)).ToDictionaryAsync(role => role.UserId, cancellationToken);
    }

    public static Task<Role> FindProjectOwnerAsync(this DbSet<Role> roles, Guid projectId, CancellationToken cancellationToken)
    {
        return roles.SingleAsync(role => role.ProjectId == projectId && role.AccessLevel == AccessLevel.Owner, cancellationToken);
    }
}