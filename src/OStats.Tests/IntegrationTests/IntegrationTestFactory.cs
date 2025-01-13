using DataServiceGrpc;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OStats.Infrastructure;
using Testcontainers.PostgreSql;

namespace OStats.Tests.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .Build();
    
    protected readonly IContainer _dataServiceContainer = new ContainerBuilder()
        .WithImage("ghcr.io/ostatsaa/data-service:main")
        .WithEnvironment("ENVIRONMENT", "DEVELOPMENT")  //as in https://github.com/OStatsAA/data-service/blob/main/dataservice/config.py
        .WithExposedPort(50051)
        .WithPortBinding(50051, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(50051))
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");
        builder.ConfigureTestServices(services =>
        {
            services.Configure<HealthCheckServiceOptions>(options =>
            {
                options.Registrations.Clear();
            });

            var descriptorType = typeof(DbContextOptions<Context>);
            var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<Context>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            services.AddMassTransitTestHarness(x =>
            {
                x.AddConsumers(typeof(Program).Assembly);
            });

            services.AddGrpcClient<DataService.DataServiceClient>(o =>
            {
                var uri = new UriBuilder("http", _dataServiceContainer.Hostname, _dataServiceContainer.GetMappedPublicPort(50051));
                o.Address = uri.Uri;
            });

            services.Configure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.Configuration = new OpenIdConnectConfiguration
                    {
                        Issuer = JwtTokenProvider.Issuer,
                    };
                    options.TokenValidationParameters.ValidIssuer = JwtTokenProvider.Issuer;
                    options.TokenValidationParameters.ValidAudience = JwtTokenProvider.Issuer;
                    options.Configuration.SigningKeys.Add(JwtTokenProvider.SecurityKey);
                }
            );
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _dataServiceContainer.StartAsync();
    }

    public async new Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _dataServiceContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}