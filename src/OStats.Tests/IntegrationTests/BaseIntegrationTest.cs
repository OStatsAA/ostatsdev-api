using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Infrastructure;

namespace OStats.Tests.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly Context context;
    protected readonly IServiceProvider serviceProvider;
    protected readonly HttpClient client;
    protected readonly ITestHarness queueHarness;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        serviceProvider = _scope.ServiceProvider;
        context = _scope.ServiceProvider.GetRequiredService<Context>();
        queueHarness = _scope.ServiceProvider.GetTestHarness();
        client = factory.CreateClient();

        SetDatabaseInitialState();
    }

    private void SetDatabaseInitialState()
    {
        context.Database.Migrate();
        if (context.Users.Any())
        {
            return;
        }

        var user = new User("Test", "test@test.com", "test_auth_identity");
        context.Add(user);

        var project = new Project(user.Id, "Test", "Test");
        context.Add(project);
        var dataset = new Dataset(user.Id, "Test", "Test", "Test");
        context.Add(dataset);
        project.LinkDataset(dataset.Id, user.Id);
        context.SaveChanges();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        context?.Dispose();
        queueHarness?.Stop();
    }
}