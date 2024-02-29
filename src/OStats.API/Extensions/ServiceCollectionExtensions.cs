using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OStats.Infrastructure;

namespace OStats.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services)
    {
        services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = "https://ostats.us.auth0.com/";
            options.Audience = "https://api.ostats.dev";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.NameIdentifier
            };
        });

        return services;
    }

    public static IServiceCollection AddMessageBroker(this IServiceCollection services)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "IntegrationTest")
        {
            return services;
        }

        services.AddMassTransit(busConfig =>
        {
            busConfig.AddConsumers(typeof(Program).Assembly);

            busConfig.AddEntityFrameworkOutbox<Context>(outboxConfig =>
            {
                outboxConfig.UsePostgres().UseBusOutbox();
            });

            busConfig.UsingRabbitMq((_, cfg) =>
            {
                var host = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_HOST");
                cfg.Host(host, "/", h =>
                {
                    h.Username(Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER"));
                    h.Password(Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS"));
                });
            });
        });
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        var scheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        };
        var requirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        };

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", scheme);
            options.AddSecurityRequirement(requirement);
        });

        return services;
    }
}