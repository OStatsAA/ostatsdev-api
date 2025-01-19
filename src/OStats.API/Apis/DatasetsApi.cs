using System.Runtime.CompilerServices;
using DataServiceGrpc;
using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.API.Filters;
using OStats.API.Queries;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API;

public static class DatasetsApi
{
    public static RouteGroupBuilder MapDatasetsApi(this RouteGroupBuilder app)
    {
        app.MapPost("/", CreateDatasetHandler).AddEndpointFilter<ValidationFilter<CreateDatasetDto>>();
        app.MapGet("/{datasetId:Guid}", GetDatasetByIdHandler);
        app.MapDelete("/{datasetId:Guid}", DeleteDatasetHandler);
        app.MapPut("/{datasetId:Guid}", UpdateDatasetHandler).AddEndpointFilter<ValidationFilter<UpdateDatasetDto>>();
        app.MapGet("/{datasetId:Guid}/data", GetDataHandler);
        app.MapPut("/{datasetId:Guid}/visibility", UpdateDatasetVisibilityHandler);
        app.MapPost("/{datasetId:Guid}/ingestdata", IngestDataHandler).AddEndpointFilter<ValidationFilter<IngestDataDto>>();
        app.MapGet("/{datasetId:Guid}/linkedprojects", GetLinkedProjectsHandler);
        app.MapPost("/{datasetId:Guid}/users", AddUserToDatasetHandler).AddEndpointFilter<ValidationFilter<AddUserToDatasetDto>>();
        app.MapDelete("/{datasetId:Guid}/users/{userId:Guid}", RemoveUserFromDatasetHandler);

        return app;
    }

    private static async Task<Results<Ok<BaseDatasetDto>, BadRequest<string>>> CreateDatasetHandler(
        [FromBody] CreateDatasetDto createDto,
        [FromServices] UserContext userContext,
        [FromServices] CreateDatasetCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new CreateDatasetCommand(userId, createDto.Title, createDto.Source, createDto.Description);
        var (result, baseDatasetDto) = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseDatasetDto) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<DatasetWithUsersDto>, NotFound>> GetDatasetByIdHandler(
        Guid datasetId,
        [FromServices] UserContext userContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var requestorUserId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var dataset = await DatasetQueries.GetDatasetByIdAsync(dbContext, requestorUserId, datasetId, cancellationToken);
        if (dataset is null)
        {
            return TypedResults.NotFound();
        }

        var users = await DatasetQueries.GetDatasetUsersAsync(dbContext, datasetId, cancellationToken);
        return TypedResults.Ok(new DatasetWithUsersDto(dataset, users));
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteDatasetHandler(
        Guid datasetId,
        [FromServices] UserContext userContext,
        [FromServices] DeleteDatasetCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new DeleteDatasetCommand(userId, datasetId);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<BaseDatasetDto>, BadRequest<string>>> UpdateDatasetHandler(
        Guid datasetId,
        [FromBody] UpdateDatasetDto updateDto,
        [FromServices] UserContext userContext,
        [FromServices] UpdateDatasetCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new UpdateDatasetCommand(datasetId, userId, updateDto.Title, updateDto.Source, updateDto.LastUpdatedAt, updateDto.Description);
        var (result, baseDatasetDto) = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseDatasetDto) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok, BadRequest<string>>> IngestDataHandler(
        Guid datasetId,
        [FromBody] IngestDataDto ingestDataDto,
        [FromServices] UserContext userContext,
        [FromServices] IngestDataCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new IngestDataCommand(userId, datasetId, ingestDataDto.Bucket, ingestDataDto.FileName);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async IAsyncEnumerable<dynamic> GetDataHandler(
        Guid datasetId,
        [FromServices] UserContext userContext,
        HttpContext context,
        Context dbContext,
        [FromServices] DataService.DataServiceClient _dataServiceClient,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var user = await userContext.GetCurrentUserAsync(cancellationToken);
        if (user is null)
        {
            yield return context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            yield break;
        }

        if (!await dbContext.DatasetsUsersAccessLevels.AnyByUserAndDatasetIdAsync(user.Id, datasetId, cancellationToken))
        {
            yield return context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            yield break;
        }

        var queryRequest = new GetDataRequest
        {
            DatasetId = datasetId.ToString(),
            Query = "SELECT * FROM data"
        };
        context.Response.ContentType = "application/stream+json";
        var call = _dataServiceClient.GetData(queryRequest, cancellationToken: cancellationToken);
        await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
        {
            yield return response.Body;
        }
    }

    private static async Task<Results<Ok, BadRequest<string>>> UpdateDatasetVisibilityHandler(
        Guid datasetId,
        [FromBody] UpdateDatasetVisibilityDto updateDto,
        [FromServices] UserContext userContext,
        [FromServices] UpdateDatasetVisibilityCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDatasetVisibilityCommand
        {
            DatasetId = datasetId,
            RequestorUserId = await userContext.GetCurrentUserIdAsync(cancellationToken),
            IsPublic = updateDto.IsPublic
        };
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<List<DatasetProjectLinkDto>>, UnauthorizedHttpResult, BadRequest>> GetLinkedProjectsHandler(
        Guid datasetId,
        [FromServices] UserContext userContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var userAccessLevel = await DatasetQueries.GetUserDatasetAccessLevelAsync(dbContext, userId, datasetId, cancellationToken);
        if (userAccessLevel < DatasetAccessLevel.ReadOnly)
        {
            return TypedResults.Unauthorized();
        }

        var linkedProjects = await DatasetQueries.GetDatasetLinkedProjectsAsync(dbContext, datasetId, cancellationToken);
        return TypedResults.Ok(linkedProjects);
    }

    private static async Task<Results<Ok, BadRequest<string>>> AddUserToDatasetHandler(
        Guid datasetId,
        [FromBody] AddUserToDatasetDto addUserToDatasetDto,
        [FromServices] UserContext userContext,
        [FromServices] AddUserToDatasetCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var userId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new AddUserToDatasetCommand(userId, datasetId, addUserToDatasetDto.UserId, addUserToDatasetDto.AccessLevel);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok, BadRequest<string>>> RemoveUserFromDatasetHandler(
        Guid datasetId,
        Guid userId,
        [FromServices] UserContext userContext,
        [FromServices] RemoveUserFromDatasetCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        var requestorId = await userContext.GetCurrentUserIdAsync(cancellationToken);
        var command = new RemoveUserFromDatasetCommand(requestorId, datasetId, userId);
        var result = await commandHandler.Handle(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }
}