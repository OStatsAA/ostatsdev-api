using MassTransit;
using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure.EntitiesConfiguration;

namespace OStats.Infrastructure;

public class Context : DbContext
{
    // DatasetAggregate db sets
    public DbSet<Dataset> Datasets { get; set; }
    public DbSet<DatasetUserAccessLevel> DatasetsUsersAccessLevels { get; set; }

    // ProjectAggregate db sets
    public DbSet<Project> Projects { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<DatasetProjectLink> DatasetsProjectsLinks { get; set; }

    // UserAggregate db sets
    public DbSet<User> Users { get; set; }

    // History db sets
    public DbSet<AggregateHistoryEntry> AggregatesHistoryEntries { get; set; }

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
        // DatasetAggregate entities configuration
        modelBuilder.ApplyConfiguration(new DatasetEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DatasetUserAccessLevelEntityConfiguration());

        // ProjectAggregate entities configuration
        modelBuilder.ApplyConfiguration(new ProjectEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DatasetProjectLinkConfiguration());

        // UserAggregate entities configuration
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());

        // History entities configuration
        modelBuilder.ApplyConfiguration(new AggregateHistoryEntryConfiguration());

        // Apply Outbox pattern configuration
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
    }

    public override int SaveChanges()
    {
        AddTimeStamps();
        return base.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        AddTimeStamps();
        return await base.SaveChangesAsync();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
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
            }
        }
    }
}