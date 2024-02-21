using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Extensions;
using OStats.API.Filters;
using OStats.API.Queries;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

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
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new CreateProjectCommand(userAuthId, createDto.Title, createDto.Description);
        var (result, baseProject) = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseProject) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<ProjectDto>, NotFound>> GetProjectByIdAsync(
        Guid projectId,
        HttpContext httpContext,
        Context dbContext)
    {
        var userAuthId = httpContext.User.GetAuthId();
        var project = await ProjectQueries.GetProjectByIdAsync(dbContext, userAuthId, projectId);
        if (project is null)
        {
            return TypedResults.NotFound();
        }

        var linkedDatasets = await ProjectQueries.GetProjectDatasetsAsync(dbContext, projectId);
        return TypedResults.Ok(new ProjectDto(project, linkedDatasets));
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

    private static async Task<Results<Ok<List<ProjectUserAndRoleDto>>, BadRequest>> GetProjectUsersAndRolesAsync(
        Guid projectId,
        HttpContext httpContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var userAuthId = httpContext.User.GetAuthId();
        var user = await dbContext.Users.FindByAuthIdentityAsync(userAuthId, cancellationToken);
        if (user is null)
        {
            return TypedResults.BadRequest();
        }

        var projectUsersAndRoles = await ProjectQueries.GetProjectUsersAndRolesAsync(dbContext, user.Id, projectId);
        return TypedResults.Ok(projectUsersAndRoles);
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