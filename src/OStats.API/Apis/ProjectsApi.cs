using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Extensions;
using OStats.API.Queries;

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
        app.MapGet("/{projectId:Guid}/users", GetProjectUsersAndRolesAsync);
        app.MapPost("/{projectId:Guid}/users", AddUserToProjectHandler);
        app.MapDelete("/{projectId:Guid}/users/{userId:Guid}", RemoveUserFromProjectHandler);

        return app;
    }

    private static async Task<Results<Ok<BaseProjectDto>, BadRequest<string>>> CreateProjectAsync(
        [FromBody] CreateProjectDto createDto,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new CreateProjectCommand(userAuthId, createDto.Title, createDto.Description);
        var (result, baseProject) = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseProject) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<ProjectDto>, BadRequest<List<ValidationFailure>>>> GetProjectByIdAsync(
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

    private static async Task<Results<Ok<BaseProjectDto>, BadRequest<string>>> UpdateProjectAsync(
        Guid projectId,
        [FromBody] UpdateProjectDto updateDto,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new UpdateProjectCommand(projectId, userAuthId, updateDto.Title, updateDto.LastUpdatedAt, updateDto.Description);
        var (result, baseProjectDto) = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseProjectDto) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<bool>, BadRequest<string>>> DeleteProjectAsync(
        Guid projectId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new DeleteProjectCommand(userAuthId, projectId);
        var result = await mediator.Send(command);
        return result.Succeeded ? TypedResults.Ok(result.Succeeded) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<List<ProjectUserAndRoleDto>>, BadRequest<List<ValidationFailure>>>> GetProjectUsersAndRolesAsync(
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

    private static async Task<Results<Ok<bool>, BadRequest<string>>> AddUserToProjectHandler(
        Guid projectId,
        [FromBody] AddUserToProjectDto addUserToProjectDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new AddUserToProjectCommand(userAuthId, projectId, addUserToProjectDto.UserId, addUserToProjectDto.AccessLevel);
        var result = await mediator.Send(command);
        return result.Succeeded ? TypedResults.Ok(result.Succeeded) : TypedResults.BadRequest(result.ErrorMessage);
    }
    private static async Task<Results<Ok<bool>, BadRequest<string>>> RemoveUserFromProjectHandler(
        Guid projectId,
        Guid userId,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new RemoveUserFromProjectCommand(userAuthId, projectId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(result.Succeeded) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<bool>, BadRequest<string>>> LinkProjectToDatasetHandler(
        Guid projectId,
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new LinkProjectToDatasetCommand(userAuthId, datasetId, projectId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(result.Succeeded) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<bool>, BadRequest<string>>> UnlinkProjectToDatasetHandler(
        Guid projectId,
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new UnlinkProjectToDatasetCommand(userAuthId, datasetId, projectId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(result.Succeeded) : TypedResults.BadRequest(result.ErrorMessage);
    }
}