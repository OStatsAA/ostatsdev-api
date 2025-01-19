using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.API.Extensions;
using OStats.API.Filters;
using OStats.API.Queries;
using OStats.Infrastructure;

namespace OStats.API;

public static class ProjectsApi
{
    public static RouteGroupBuilder MapProjectsApi(this RouteGroupBuilder app)
    {
        app.MapPost("/", CreateProjectAsync).AddEndpointFilter<ValidationFilter<CreateProjectDto>>();
        app.MapGet("/{projectId:Guid}", GetProjectByIdAsync);
        app.MapPut("/{projectId:Guid}", UpdateProjectAsync).AddEndpointFilter<ValidationFilter<UpdateProjectDto>>();
        app.MapDelete("/{projectId:Guid}", DeleteProjectAsync);
        app.MapPost("/{projectId:Guid}/linkdataset/{datasetId:Guid}", LinkProjectToDatasetHandler);
        app.MapDelete("/{projectId:Guid}/linkdataset/{datasetId:Guid}", UnlinkProjectToDatasetHandler);
        app.MapGet("/{projectId:Guid}/users", GetProjectUsersAndRolesAsync);
        app.MapPost("/{projectId:Guid}/users", AddUserToProjectHandler).AddEndpointFilter<ValidationFilter<AddUserToProjectDto>>();
        app.MapDelete("/{projectId:Guid}/users/{userId:Guid}", RemoveUserFromProjectHandler);

        return app;
    }

    private static async Task<Results<Ok<BaseProjectDto>, BadRequest<string>>> CreateProjectAsync(
        [FromBody] CreateProjectDto createDto,
        [FromServices] UserContext userContext,
        [FromServices] CreateProjectCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new CreateProjectCommand(userId, createDto.Title, createDto.Description);
        var (result, baseProject) = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseProject) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<ProjectDto>, NotFound>> GetProjectByIdAsync(
        Guid projectId,
        [FromServices] UserContext userContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var project = await ProjectQueries.GetProjectByIdAsync(dbContext, userId, projectId, cancellationToken);
        if (project is null)
        {
            return TypedResults.NotFound();
        }

        var linkedDatasets = await ProjectQueries.GetProjectDatasetsAsync(dbContext, projectId, cancellationToken);
        return TypedResults.Ok(new ProjectDto(project, linkedDatasets));
    }

    private static async Task<Results<Ok<BaseProjectDto>, BadRequest<string>>> UpdateProjectAsync(
        Guid projectId,
        [FromBody] UpdateProjectDto updateDto,
        [FromServices] UserContext userContext,
        [FromServices] UpdateProjectCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new UpdateProjectCommand(projectId, userId, updateDto.Title, updateDto.LastUpdatedAt, updateDto.Description);
        var (result, baseProjectDto) = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseProjectDto) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteProjectAsync(
        Guid projectId,
        [FromServices] UserContext userContext,
        [FromServices] DeleteProjectCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new DeleteProjectCommand(userId, projectId);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<List<ProjectUserAndRoleDto>>, BadRequest>> GetProjectUsersAndRolesAsync(
        Guid projectId,
        [FromServices] UserContext userContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var user = await userContext.GetCurrentUserAsync(cancellationToken);
        if (user is null)
        {
            return TypedResults.BadRequest();
        }

        var projectUsersAndRoles = await ProjectQueries.GetProjectUsersAndRolesAsync(dbContext, user.Id, projectId, cancellationToken);
        return TypedResults.Ok(projectUsersAndRoles);
    }

    private static async Task<Results<Ok, BadRequest<string>>> AddUserToProjectHandler(
        Guid projectId,
        [FromBody] AddUserToProjectDto addUserToProjectDto,
        [FromServices] UserContext userContext,
        [FromServices] AddUserToProjectCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new AddUserToProjectCommand(userId, projectId, addUserToProjectDto.UserId, addUserToProjectDto.AccessLevel);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok, BadRequest<string>>> RemoveUserFromProjectHandler(
        Guid projectId,
        Guid userId,
        [FromServices] UserContext userContext,
        [FromServices] RemoveUserFromProjectCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var requestorId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new RemoveUserFromProjectCommand(requestorId, projectId, userId);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok, BadRequest<string>>> LinkProjectToDatasetHandler(
        Guid projectId,
        Guid datasetId,
        [FromServices] UserContext userContext,
        [FromServices] LinkProjectToDatasetCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new LinkProjectToDatasetCommand(userId, datasetId, projectId);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok, BadRequest<string>>> UnlinkProjectToDatasetHandler(
        Guid projectId,
        Guid datasetId,
        [FromServices] UserContext userContext,
        [FromServices] UnlinkProjectToDatasetCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new UnlinkProjectToDatasetCommand(userId, datasetId, projectId);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }
}