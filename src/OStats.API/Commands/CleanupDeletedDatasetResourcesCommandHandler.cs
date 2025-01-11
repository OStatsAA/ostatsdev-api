using DataServiceGrpc;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;
public sealed class CleanupDeletedDatasetResourcesCommandHandler : CommandHandler<CleanupDeletedDatasetResourcesCommand, DomainOperationResult>
{
    private readonly DataService.DataServiceClient _dataServiceClient;
    public CleanupDeletedDatasetResourcesCommandHandler(Context context, IPublishEndpoint publishEndpoint, DataService.DataServiceClient dataServiceClient) : base(context, publishEndpoint)
    {
        _dataServiceClient = dataServiceClient;
    }

    public override async Task<DomainOperationResult> Handle(CleanupDeletedDatasetResourcesCommand command, CancellationToken cancellationToken)
    {
        var databaseCleanup = CleanupDatabaseAsync(command.DatasetId, cancellationToken);
        var storageCleanup = CleanupStorageAsync(command.DatasetId, cancellationToken);

        await Task.WhenAll(databaseCleanup, storageCleanup);

        if (databaseCleanup.Result.Succeeded && storageCleanup.Result.Succeeded)
        {
            return DomainOperationResult.Success;
        }

        return DomainOperationResult.Failure("Failed to cleanup dataset resources");
    }

    private async Task<DomainOperationResult> CleanupDatabaseAsync(Guid datasetId, CancellationToken cancellationToken)
    {
        var dataset = await _context.Datasets
            .Include(dataset => dataset.DatasetUserAccessLevels)
            .FirstOrDefaultAsync(dataset => dataset.Id == datasetId, cancellationToken: cancellationToken);

        if (dataset is null)
        {
            return DomainOperationResult.NoActionTaken("Dataset is already deleted from database");
        }

        if (!dataset.IsDeleted)
        {
            return DomainOperationResult.Failure("Dataset is not deleted");
        }

        _context.Remove(dataset);
        await SaveCommandHandlerChangesAsync(cancellationToken);
        return DomainOperationResult.Success;
    }

    private async Task<DomainOperationResult> CleanupStorageAsync(Guid datasetId, CancellationToken cancellationToken)
    {
        var request = new DeleteDatasetRequest { DatasetId = datasetId.ToString() };
        var response = await _dataServiceClient.DeleteDatasetAsync(request, cancellationToken: cancellationToken);

        if (response.Success)
        {
            return DomainOperationResult.Success;
        }

        return DomainOperationResult.Failure("Failed to delete dataset from storage");
    }
}