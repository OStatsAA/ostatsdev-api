using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Extensions;
using OStats.API.Filters;
using OStats.API.Queries;
using OStats.Infrastructure;

namespace OStats.API;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsersApi(this RouteGroupBuilder app)
    {
        app.MapGet("/", PeopleSearchHandler);
        app.MapPost("/", CreateUserAsync).AddEndpointFilter<ValidationFilter<CreateUserDto>>();
        app.MapGet("/{userId:Guid}", GetUserByIdAsync);
        app.MapGet("/{userId:Guid}/projects", GetUserProjectsAsync);
        app.MapGet("/{userId:Guid}/datasets", GetUserDatasetsHandler);

        return app;
    }

    private static async Task<Ok<List<BaseUserDto>>> PeopleSearchHandler(
        [FromQuery] string search,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var users = await UserQueries.SearchUsersAsync(dbContext, search, cancellationToken);
        return TypedResults.Ok(users);
    }

    private static async Task<Results<Ok<BaseUserDto>, BadRequest<string>>> CreateUserAsync(
        [FromBody] CreateUserDto createDto,
        HttpContext context,
        [FromServices] CreateUserCommandHandler commandHandler,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.GetUserAuthId();
        var user = await UserQueries.GetUserByAuthIdAsync(dbContext, userAuthId, cancellationToken);
        if (user is not null)
        {
            return TypedResults.Ok(user);
        }

        var command = new CreateUserCommand(createDto.Name, createDto.Email, userAuthId);
        var (result, baseUserDto) = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseUserDto) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<BaseUserDto>, NotFound>> GetUserByIdAsync(
        [FromRoute] Guid userId,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var user = await UserQueries.GetUserByIdAsync(dbContext, userId, cancellationToken);
        return user is not null ? TypedResults.Ok(user) : TypedResults.NotFound();
    }

    private static async Task<Ok<List<UserProjectDto>>> GetUserProjectsAsync(
        [FromRoute] Guid userId,
        HttpContext httpContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var userAuthId = httpContext.GetUserAuthId();
        var userProjects = await UserQueries.GetUserProjectsAsync(dbContext, userAuthId, userId, cancellationToken);
        return TypedResults.Ok(userProjects);
    }

    private static async Task<Ok<List<UserDatasetDto>>> GetUserDatasetsHandler(
        [FromRoute] Guid userId,
        HttpContext context,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.GetUserAuthId();
        var userDatasets = await UserQueries.GetUserDatasetsAsync(dbContext, userAuthId, userId, cancellationToken);
        return TypedResults.Ok(userDatasets);
    }
}