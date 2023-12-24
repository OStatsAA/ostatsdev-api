using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OStats.API.Commands;
using OStats.API.Dtos;
using OStats.API.Queries;
using OStats.Domain.Aggregates.DatasetAggregate;

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

        return app;
    }

    public static async Task<Results<Ok<Dataset>, BadRequest<List<ValidationFailure>>>> CreateDatasetHandler(
        [FromBody] CreateDatasetCommandDto createDto,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
        var command = new CreateDatasetCommand(userAuthId, createDto.Title, createDto.Source, createDto.Description);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest(commandResult.ValidationFailures);
        }

        return TypedResults.Ok(commandResult.Value);
    }

    public static async Task<Results<Ok<Dataset>, BadRequest<List<ValidationFailure>>>> GetDatasetByIdHandler(
        Guid datasetId,
        HttpContext context,
        [FromServices] IMediator mediator)
    {
        var userAuthId = GetUserAuthId(context);
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
        var userAuthId = GetUserAuthId(context);
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
        var userAuthId = GetUserAuthId(context);
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
        var userAuthId = GetUserAuthId(context);
        var command = new IngestDataCommand(userAuthId, datasetId, ingestDataDto.Bucket, ingestDataDto.FileName);
        var commandResult = await mediator.Send(command);
        if (!commandResult.Success)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(commandResult.Success);
    }

    private static string GetUserAuthId(HttpContext context)
    {
        return context.User.Identity?.Name ?? throw new ArgumentNullException(nameof(context.User.Identity));
    }
}