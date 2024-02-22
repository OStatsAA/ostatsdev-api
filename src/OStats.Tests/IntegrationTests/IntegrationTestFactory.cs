using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OStats.Infrastructure;
using Testcontainers.PostgreSql;

namespace OStats.Tests.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
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

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}