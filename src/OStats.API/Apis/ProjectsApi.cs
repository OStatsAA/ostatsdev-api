using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Extensions;
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
        app.MapPost("/{projectId:Guid}/linkdataset/{datasetId:Guid}", LinkProjectToDatasetHandler);
        app.MapDelete("/{projectId:Guid}/linkdataset/{datasetId:Guid}", UnlinkProjectToDatasetHandler);
        app.MapGet("/{projectId:Guid}/usersroles", GetProjectUsersAndRolesAsync);

        return app;
    }

    public static async Task<Results<Ok<Project>, BadRequest<List<ValidationFailure>>>> CreateProjectAsync(
        [FromBody] CreateProjectDto createDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new CreateProjectCommand(userAuthId, createDto.Title, createDto.Description);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<ProjectDto>, BadRequest<List<ValidationFailure>>>> GetProjectByIdAsync(
        Guid projectId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
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
        [FromBody] UpdateProjectDto updateDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
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
        var userAuthId = context.User.GetAuthId();
        var command = new DeleteProjectCommand(userAuthId, projectId);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<List<ProjectUserAndRoleDto>>, BadRequest<List<ValidationFailure>>>> GetProjectUsersAndRolesAsync(
        Guid projectId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var query = new ProjectUsersAndRolesQuery(userAuthId, projectId);
        var queryResult = await mediator.Send(query);

        if (!queryResult.Success)
        {
            return TypedResults.BadRequest(queryResult.ValidationFailures);
        }

        return TypedResults.Ok(queryResult.Value);
    }

    public static async Task<Results<Ok<bool>, BadRequest<List<ValidationFailure>>>> LinkProjectToDatasetHandler(
        Guid projectId,
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new LinkProjectToDatasetCommand(userAuthId, datasetId, projectId);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<bool>, BadRequest<List<ValidationFailure>>>> UnlinkProjectToDatasetHandler(
        Guid projectId,
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new UnlinkProjectToDatasetCommand(userAuthId, datasetId, projectId);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }
}