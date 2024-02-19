using System.Text.Json;
using DataServiceGrpc;
using FluentValidation.Results;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Extensions;
using OStats.API.Queries;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API;

public static class DatasetsApi
{
    public static RouteGroupBuilder MapDatasetsApi(this RouteGroupBuilder app)
    {
        app.MapPost("/", CreateDatasetHandler);
        app.MapGet("/{datasetId:Guid}", GetDatasetByIdHandler);
        app.MapDelete("/{datasetId:Guid}", DeleteDatasetHandler);
        app.MapPut("/{datasetId:Guid}", UpdateDatasetHandler);
        app.MapGet("/{datasetId:Guid}/getdata", GetDataHandler);
        app.MapPost("/{datasetId:Guid}/ingestdata", IngestDataHandler);
        app.MapGet("/{datasetId:Guid}/linkedprojects", GetLinkedProjectsHandler);
        app.MapPost("/{datasetId:Guid}/users", AddUserToDatasetHandler);
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

    private static async Task<Results<Ok<DatasetWithUsersDto>, BadRequest<List<ValidationFailure>>>> GetDatasetByIdHandler(
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new DatasetByIdQuery(userAuthId, datasetId);
        var commandResult = await mediator.Send(command);
        return commandResult.Success ? TypedResults.Ok(commandResult.Value) : TypedResults.BadRequest(commandResult.ValidationFailures);
    }

    private static async Task<Results<Ok<bool>, BadRequest<string>>> DeleteDatasetHandler(
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new DeleteDatasetCommand(userAuthId, datasetId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(result.Succeeded) : TypedResults.BadRequest(result.ErrorMessage);
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

    private static async Task<Results<Ok<bool>, BadRequest>> IngestDataHandler(
        Guid datasetId,
        [FromBody] IngestDataDto ingestDataDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new IngestDataCommand(userAuthId, datasetId, ingestDataDto.Bucket, ingestDataDto.FileName);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(commandResult.Success);
    }

    private static async IAsyncEnumerable<dynamic> GetDataHandler(
        Guid datasetId,
        HttpContext context,
        Context dbContext,
        [FromServices] DataService.DataServiceClient _dataServiceClient,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindByAuthIdentityAsync(context.User.GetAuthId(), cancellationToken);
        if (user is null)
        {
            context.Response.StatusCode = TypedResults.BadRequest().StatusCode;
            yield break;
        }

        if (!await dbContext.DatasetsUsersAccessLevels.AnyAsync(accesses => accesses.UserId == user.Id && accesses.DatasetId == datasetId))
        {
            context.Response.StatusCode = TypedResults.Unauthorized().StatusCode;
            yield break;
        }

        var queryRequest = new GetDataRequest()
        {
            DatasetId = datasetId.ToString(),
            Query = "SELECT * FROM data"
        };
        context.Response.ContentType = "application/stream+json";
        var call = _dataServiceClient.GetData(queryRequest, cancellationToken: cancellationToken);
        await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
        {
            var json = JsonSerializer.Deserialize<dynamic>(response.Body);
            if (json is not null)
            {
                yield return json;
            }
        }
    }

    private static async Task<Results<Ok<List<DatasetProjectLinkDto>>, BadRequest<List<ValidationFailure>>>> GetLinkedProjectsHandler(
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new DatasetLinkedProjectsQuery(userAuthId, datasetId);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    private static async Task<Results<Ok<bool>, BadRequest<string>>> AddUserToDatasetHandler(
        Guid datasetId,
        [FromBody] AddUserToDatasetDto addUserToDatasetDto,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new AddUserToDatasetCommand(userAuthId, datasetId, addUserToDatasetDto.UserId, addUserToDatasetDto.AccessLevel);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(result.Succeeded) : TypedResults.BadRequest(result.ErrorMessage);
    }

    private static async Task<Results<Ok<bool>, BadRequest<string>>> RemoveUserFromDatasetHandler(
        Guid datasetId,
        Guid userId,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new RemoveUserFromDatasetCommand(userAuthId, datasetId, userId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? TypedResults.Ok(result.Succeeded) : TypedResults.BadRequest(result.ErrorMessage);
    }
}