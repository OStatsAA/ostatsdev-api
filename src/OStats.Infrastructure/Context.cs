using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure.EntitiesConfiguration;

namespace OStats.Infrastructure;

public sealed class Context : DbContext
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

    public Context(DbContextOptions options) : base(options) 
    {
        ChangeTracker.StateChanged += UpdateTimestamps;
        ChangeTracker.Tracked += UpdateTimestamps;
    }

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

    private static void UpdateTimestamps(object? sender, EntityEntryEventArgs e)
    {
        if(e.Entry.Entity is not Entity entity)
        {
            return;
        }

        switch (e.Entry.State)
        {
            case EntityState.Added:
                entity.CreatedAt = DateTime.UtcNow;
                entity.LastUpdatedAt = entity.CreatedAt;
                break;
            case EntityState.Modified or EntityState.Deleted:
                entity.LastUpdatedAt = DateTime.UtcNow;
                break;
        }
    }
}