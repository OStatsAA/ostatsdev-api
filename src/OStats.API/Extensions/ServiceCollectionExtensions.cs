using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OStats.API.Commands;
using OStats.API.Queries;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Infrastructure.Repositories;

namespace OStats.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddScoped<IProjectQueries, ProjectQueries>();
        services.AddScoped<IUserQueries, UserQueries>();

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateProjectCommand>, CreateProjectCommandValidator>();
        services.AddScoped<IValidator<UpdateProjectCommand>, UpdateProjectCommandValidator>();
        services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();
        services.AddScoped<IValidator<AddDatasetConfigurationCommand>, AddDatasetConfigurationCommandValidator>();
        services.AddScoped<IValidator<RemoveDatasetConfigurationCommand>, RemoveDatasetConfigurationCommandValidator>();

        return services;
    }

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