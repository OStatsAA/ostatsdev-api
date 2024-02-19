using MediatR;
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
    protected readonly ISender sender;
    protected readonly Context context;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        context = _scope.ServiceProvider.GetRequiredService<Context>();

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

        foreach (var change in context.ChangeTracker.Entries())
        {
            Console.WriteLine(change.Entity.ToString());
        } 
    

        context.SaveChanges();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        context?.Dispose();
    }
}