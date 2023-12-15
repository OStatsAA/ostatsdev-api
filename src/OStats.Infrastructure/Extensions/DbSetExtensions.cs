using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Infrastructure.Extensions;

public static class DbSetExtensions
{
    public static Task<Project?> FindByDatasetConfigurationIdAsync(this DbSet<Project> projects, Guid datasetConfigId)
    {
        return projects.Include(project => project.DatasetsConfigs)
                       .Where(project => project.DatasetsConfigs.Select(dc => dc.Id).Contains(datasetConfigId))
                       .SingleOrDefaultAsync();
    }

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