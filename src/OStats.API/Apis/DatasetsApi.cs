using System.Runtime.CompilerServices;
using DataServiceGrpc;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Extensions;
using OStats.API.Filters;
using OStats.API.Queries;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

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
        app.MapPost("/{datasetId:Guid}/ingestdata", IngestDataHandler).AddEndpointFilter<ValidationFilter<IngestDataDto>>();
        app.MapGet("/{datasetId:Guid}/linkedprojects", GetLinkedProjectsHandler);
        app.MapPost("/{datasetId:Guid}/users", AddUserToDatasetHandler).AddEndpointFilter<ValidationFilter<AddUserToDatasetDto>>();
        app.MapDelete("/{datasetId:Guid}/users/{userId:Guid}", RemoveUserFromDatasetHandler);

        return app;
    }

    private static async Task<Results<Ok<BaseDatasetDto>, BadRequest<string>>> CreateDatasetHandler(
        [FromBody] CreateDatasetDto createDto,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new CreateDatasetCommand(userAuthId, createDto.Title, createDto.Source, createDto.Description);
        var (result, baseDatasetDto) = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseDatasetDto) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<DatasetWithUsersDto>, NotFound>> GetDatasetByIdHandler(
        Guid datasetId,
        HttpContext httpContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var userAuthId = httpContext.User.GetAuthId();
        var dataset = await DatasetQueries.GetDatasetByIdAsync(dbContext, userAuthId, datasetId, cancellationToken);
        if (dataset is null)
        {
            return TypedResults.NotFound();
        }

        var users = await DatasetQueries.GetDatasetUsersAsync(dbContext, datasetId, cancellationToken);
        return TypedResults.Ok(new DatasetWithUsersDto(dataset, users));
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteDatasetHandler(
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new DeleteDatasetCommand(userAuthId, datasetId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<BaseDatasetDto>, BadRequest<string>>> UpdateDatasetHandler(
        Guid datasetId,
        [FromBody] UpdateDatasetDto updateDto,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new UpdateDatasetCommand(datasetId, userAuthId, updateDto.Title, updateDto.Source, updateDto.LastUpdatedAt, updateDto.Description);
        var (result, baseDatasetDto) = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(baseDatasetDto) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok, BadRequest<string>>> IngestDataHandler(
        Guid datasetId,
        [FromBody] IngestDataDto ingestDataDto,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new IngestDataCommand(userAuthId, datasetId, ingestDataDto.Bucket, ingestDataDto.FileName);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async IAsyncEnumerable<dynamic> GetDataHandler(
        Guid datasetId,
        HttpContext context,
        Context dbContext,
        [FromServices] DataService.DataServiceClient _dataServiceClient,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindByAuthIdentityAsync(context.User.GetAuthId(), cancellationToken);
        if (user is null)
        {
            yield return context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            yield break;
        }

        if (!await dbContext.DatasetsUsersAccessLevels.AnyAsync(accesses => accesses.UserId == user.Id && accesses.DatasetId == datasetId, cancellationToken))
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

    private static async Task<Results<Ok<List<DatasetProjectLinkDto>>, UnauthorizedHttpResult, BadRequest>> GetLinkedProjectsHandler(
        Guid datasetId,
        HttpContext httpContext,
        Context dbContext,
        CancellationToken cancellationToken)
    {
        var userAuthId = httpContext.User.GetAuthId();
        var userAccessLevel = await DatasetQueries.GetUserDatasetAccessLevelAsync(dbContext, userAuthId, datasetId, cancellationToken);
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
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new AddUserToDatasetCommand(userAuthId, datasetId, addUserToDatasetDto.UserId, addUserToDatasetDto.AccessLevel);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok, BadRequest<string>>> RemoveUserFromDatasetHandler(
        Guid datasetId,
        Guid userId,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new RemoveUserFromDatasetCommand(userAuthId, datasetId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result.ErrorMessage);
    }
}