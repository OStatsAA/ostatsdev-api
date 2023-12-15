using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure.EntitiesConfiguration;

namespace OStats.Infrastructure;

public class Context : DbContext
{
    // ProjectAggregates db sets
    public DbSet<Project> Projects { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<DatasetConfiguration> DatasetsConfigurations { get; set; }

    // UserAggregates db sets
    public DbSet<User> Users { get; set; }

    public Context() { }

    public Context(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(BuildConnectionString());
        }
    }

    private static string BuildConnectionString()
    {
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        return @$"Host={host};Username={user};Password={password};Database={database}";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ProjectAggregate entities configuration
        modelBuilder.ApplyConfiguration(new ProjectEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DatasetConfigurationEntityConfiguration());

        // UserAggregate entities configuration
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
    }

    public override int SaveChanges()
    {
        AddTimeStamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimeStamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimeStamps()
    {
        foreach (var entityEntry in ChangeTracker.Entries<Entity>().Where(entry => entry.State != EntityState.Unchanged))
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Entity.CreatedAt = DateTime.UtcNow;
                entityEntry.Entity.LastUpdatedAt = DateTime.UtcNow;
                continue;
            }
            if (entityEntry.State == EntityState.Modified || entityEntry.State == EntityState.Deleted)
            {
                entityEntry.Entity.LastUpdatedAt = DateTime.UtcNow;
                continue;
            }
        }
    }
}