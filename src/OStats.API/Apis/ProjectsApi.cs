using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Queries;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API;

public static class ProjectsApi
{
    public static RouteGroupBuilder MapProjectsApi(this RouteGroupBuilder app)
    {
        app.MapPost("/", CreateProjectAsync);
        app.MapGet("/{projectId:Guid}", GetProjectByIdAsync);
        app.MapPut("/{projectId:Guid}", UpdateProjectAsync);
        app.MapDelete("/{projectId:Guid}", DeleteProjectAsync);
        app.MapPost("/{projectId:Guid}/datasetconfiguration", AddDatasetConfigurationToProjectAsync);
        app.MapPost("/{projectId:Guid}/datasetconfiguration/{datasetConfigId:Guid}", DeleteDatasetConfigurationFromProjectAsync);
        app.MapGet("/{projectId:Guid}/usersroles", GetProjectUsersAndRolesAsync);

        return app;
    }

    public static async Task<Results<Ok<Project>, BadRequest<List<ValidationFailure>>>> CreateProjectAsync(
        [FromBody] CreateProjectCommandDto createDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var command = new CreateProjectCommand(userAuthId, createDto.Title, createDto.Description);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<Project>, BadRequest<List<ValidationFailure>>>> GetProjectByIdAsync(
        Guid projectId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var query = new ProjectByIdQuery(userAuthId, projectId);
        var queryResult = await mediator.Send(query);

        if (!queryResult.Success)
        {
            return TypedResults.BadRequest(queryResult.ValidationFailures);
        }

        return TypedResults.Ok(queryResult.Value);
    }

    public static async Task<Results<Ok<Project>, BadRequest<List<ValidationFailure>>>> UpdateProjectAsync(
        Guid projectId,
        [FromBody] UpdateProjectCommandDto updateDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var command = new UpdateProjectCommand(projectId, userAuthId, updateDto.Title, updateDto.LastUpdatedAt, updateDto.Description);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<bool>, BadRequest<List<ValidationFailure>>>> DeleteProjectAsync(
        Guid projectId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var command = new DeleteProjectCommand(userAuthId, projectId);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<DatasetConfiguration>, BadRequest<List<ValidationFailure>>>> AddDatasetConfigurationToProjectAsync(
        Guid projectId,
        [FromBody] AddDatasetConfigurationCommandDto addDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var command = new AddDatasetConfigurationCommand(userAuthId, projectId, addDto.Title, addDto.Source, addDto.Description);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<bool>, BadRequest<List<ValidationFailure>>>> DeleteDatasetConfigurationFromProjectAsync(
        Guid projectId,
        Guid datasetConfigId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var command = new RemoveDatasetConfigurationCommand(userAuthId, projectId, datasetConfigId);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<List<ProjectUserAndRoleDto>>, UnauthorizedHttpResult, BadRequest<List<ValidationFailure>>>> GetProjectUsersAndRolesAsync(
        Guid projectId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var query = new ProjectUsersAndRolesQuery(userAuthId, projectId);
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