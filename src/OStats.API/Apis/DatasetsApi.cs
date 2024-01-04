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
using OStats.Domain.Aggregates.DatasetAggregate;
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
        app.MapPost("/{datasetId:Guid}/ingestdata", IngestDataHandler);
        app.MapGet("/{datasetId:Guid}/getdata", GetDataHandler);

        return app;
    }

    public static async Task<Results<Ok<Dataset>, BadRequest<List<ValidationFailure>>>> CreateDatasetHandler(
        [FromBody] CreateDatasetCommandDto createDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new CreateDatasetCommand(userAuthId, createDto.Title, createDto.Source, createDto.Description);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<DatasetWithUsersDto>, BadRequest<List<ValidationFailure>>>> GetDatasetByIdHandler(
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new DatasetByIdQuery(userAuthId, datasetId);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<bool>, BadRequest<List<ValidationFailure>>>> DeleteDatasetHandler(
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new DeleteDatasetCommand(userAuthId, datasetId);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<Dataset>, BadRequest<List<ValidationFailure>>>> UpdateDatasetHandler(
        Guid datasetId,
        [FromBody] UpdateDatasetCommand updateDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = context.User.GetAuthId();
        var command = new UpdateDatasetCommand(datasetId, userAuthId, updateDto.Title, updateDto.Source, updateDto.LastUpdatedAt, updateDto.Description);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<bool>, BadRequest>> IngestDataHandler(
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

    public static async IAsyncEnumerable<dynamic> GetDataHandler(
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
}