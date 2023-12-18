using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Infrastructure.Extensions;

public static class DbSetExtensions
{
    public static Task<User?> FindByAuthIdentityAsync(this DbSet<User> users, string authIdentity, CancellationToken cancellationToken)
    {
        return users.Where(user => user.AuthIdentity == authIdentity)
                    .SingleOrDefaultAsync(cancellationToken);
    }

    public static Task<bool> AnyByAuthIdentityAsync(this DbSet<User> users, string authIdentity, CancellationToken cancellationToken)
    {
        return users.Where(user => user.AuthIdentity == authIdentity).AnyAsync(cancellationToken);
    }
}