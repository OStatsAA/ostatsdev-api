using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        context.Database.MigrateAsync().Wait();
        queueHarness = _scope.ServiceProvider.GetTestHarness();
        client = factory.CreateClient();
        queueHarness.Start();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        context?.Dispose();
        queueHarness?.Stop();
    }
}