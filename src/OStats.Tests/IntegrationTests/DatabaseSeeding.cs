using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests;

public static class DatabaseSeeding
{
    public static Func<DbContext, bool, CancellationToken, Task> SeedAsync = async (context, _, cancellationToken) =>
    {
        if (await context.Set<User>().AnyAsync(cancellationToken))
        {
            return;
        }

        await SeedUsersAsync(context, cancellationToken);
        await SeedProjectsAsync(context, cancellationToken);
        await SeedDatasetsAsync(context, cancellationToken);
        await SeedProjectDatasetLinkAsync(context, cancellationToken);
    };

    private static async Task SeedUsersAsync(DbContext context, CancellationToken cancellationToken)
    {
        var user = new User("Test", "test@test.com", "test_auth_identity");
        await context.Set<User>().AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public static async Task SeedProjectsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var user = await context.Set<User>().FirstAsync(cancellationToken);
        var project = new Project(user.Id, "Test", "Test");
        await context.Set<Project>().AddAsync(project, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public static async Task SeedDatasetsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var user = await context.Set<User>().FirstAsync(cancellationToken);
        var dataset = new Dataset(user.Id, "Test", "Test", "Test");
        await context.Set<Dataset>().AddAsync(dataset, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public static async Task SeedProjectDatasetLinkAsync(DbContext context, CancellationToken cancellationToken)
    {
        var user = await context.Set<User>().FirstAsync(cancellationToken);
        var project = await context.Set<Project>().FirstAsync(cancellationToken);
        var dataset = await context.Set<Dataset>().FirstAsync(cancellationToken);
        project.LinkDataset(dataset.Id, user.Id);
        await context.SaveChangesAsync(cancellationToken);
    }
}