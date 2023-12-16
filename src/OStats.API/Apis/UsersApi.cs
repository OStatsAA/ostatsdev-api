using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Queries;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API;

public static class UsersApi
{
    public static RouteGroupBuilder MapUsersApi(this RouteGroupBuilder app)
    {
        app.MapPost("/", CreateUserAsync);
        app.MapGet("/{userId:Guid}", GetUserByIdAsync);
        app.MapGet("/{userId:Guid}/projects", GetUserProjectsAsync);

        return app;
    }

    public static async Task<Results<Ok<User>, BadRequest<List<ValidationFailure>>>> CreateUserAsync(
        [FromBody] CreateUserCommandDto createDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var query = new UserByAuthIdQuery(userAuthId);
        var queryResult = await mediator.Send(query);
        if (queryResult.Success)
        {
            return TypedResults.Ok(queryResult.Value);
        }

        var command = new CreateUserCommand(createDto.Name, createDto.Email, userAuthId);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<User>, BadRequest<List<ValidationFailure>>>> GetUserByIdAsync(
        [FromRoute] Guid userId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var query = new UserByIdQuery(userAuthId, userId);
        var queryResult = await mediator.Send(query);

        if (!queryResult.Success)
        {
            return TypedResults.BadRequest(queryResult.ValidationFailures);
        }

        return TypedResults.Ok(queryResult.Value);
    }

    public static async Task<Results<Ok<List<UserProjectDto>>, BadRequest<List<ValidationFailure>>>> GetUserProjectsAsync(
        [FromRoute] Guid userId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var query = new UserProjectsWithRoleQuery(userAuthId, userId);
        var queryResult = await mediator.Send(query);

        if (!queryResult.Success)
        {
            return TypedResults.BadRequest(queryResult.ValidationFailures);
        }

        return TypedResults.Ok(queryResult.Value);
    }

    private static string GetUserAuthId(HttpContext context)
    {
        return context.User.Identity?.Name ?? throw new ArgumentNullException(nameof(context.User.Identity));
    }
}