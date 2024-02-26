using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.DatasetAggregate;
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
}