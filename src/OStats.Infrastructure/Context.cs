using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure.EntitiesConfiguration;

namespace OStats.Infrastructure;

public class Context : DbContext, IUnitOfWork
{
    // ProjectAggregates db sets
    public DbSet<Project> Projects { get; set; }
    public DbSet<Role> Roles { get; set; }

    // UserAggregates db sets
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(BuildConnectionString());
    }

    private static string BuildConnectionString()
    {
        var envVariables = Environment.GetEnvironmentVariables();
        var host = envVariables["POSTGRES_HOST"];
        var database = envVariables["POSTGRES_DB"];
        var user = envVariables["POSTGRES_USER"];
        var password = envVariables["POSTGRES_PASSWORD"];
        return @$"Host={host};Username={user};Password={password};Database={database}";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ProjectAggregate entities configuration
        modelBuilder.ApplyConfiguration(new ProjectEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());

        // UserAggregate entities configuration
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        //var changedEntries = this.ChangeTracker.Entries<Entity>().Where(entry => entry.State != EntityState.Unchanged);
        return 1 == (await base.SaveChangesAsync(cancellationToken));
    }
}