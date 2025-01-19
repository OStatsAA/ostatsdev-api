using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.API.Extensions;
using OStats.API.Filters;
using OStats.API.Middlewares;
using OStats.API.Queries;
using OStats.Infrastructure;

namespace OStats.API;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsersApi(this RouteGroupBuilder app)
    {
        app.MapGet("/", PeopleSearchHandler);
        app.MapPost("/", CreateUserAsync)
            .WithMetadata(UserContextMiddleware.ByPassUserContextMiddleware)
            .AddEndpointFilter<ValidationFilter<CreateUserDto>>();
        app.MapGet("/{userId:Guid}", GetUserByIdAsync);
        app.MapGet("/{userId:Guid}/projects", GetUserProjectsAsync);
        app.MapGet("/{userId:Guid}/datasets", GetUserDatasetsHandler);
        app.MapDelete("/{userId:Guid}", DeleteUserHandler);

        return app;
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteUserHandler(
        [FromRoute] Guid userId,
        [FromServices] UserContext userContext,
        [FromServices] DeleteUserCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var requestorId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new DeleteUserCommand { UserId = userId, RequestorUserId = requestorId };
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
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
        HttpContext httpContext,
        [FromServices] CreateUserCommandHandler commandHandler,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var userAuthId = httpContext.GetUserAuthId();
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
        [FromServices] UserContext userContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var requestorId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var userProjects = await UserQueries.GetUserProjectsAsync(dbContext, requestorId, userId, cancellationToken);
        return TypedResults.Ok(userProjects);
    }

    private static async Task<Ok<List<UserDatasetDto>>> GetUserDatasetsHandler(
        [FromRoute] Guid userId,
        [FromServices] UserContext userContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var requestorUserId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var userDatasets = await UserQueries.GetUserDatasetsAsync(dbContext, requestorUserId, userId, cancellationToken);
        return TypedResults.Ok(userDatasets);
    }
}