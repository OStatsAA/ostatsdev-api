using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Domain.Common;

namespace OStats.Infrastructure;

public sealed class Context : DbContext
{
    // DatasetAggregate db sets
    public required DbSet<Dataset> Datasets { get; set; }
    public required DbSet<DatasetUserAccessLevel> DatasetsUsersAccessLevels { get; set; }

    // ProjectAggregate db sets
    public required DbSet<Project> Projects { get; set; }
    public required DbSet<Role> Roles { get; set; }
    public required DbSet<DatasetProjectLink> DatasetsProjectsLinks { get; set; }

    // UserAggregate db sets
    public required DbSet<User> Users { get; set; }

    // History db sets
    public required DbSet<AggregateHistoryEntry> AggregatesHistoryEntries { get; set; }

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
        
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