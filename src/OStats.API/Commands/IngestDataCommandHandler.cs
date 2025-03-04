using DataServiceGrpc;
using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class IngestDataCommandHandler : CommandHandler<IngestDataCommand, DomainOperationResult>
{
    private readonly DataService.DataServiceClient _dataServiceClient;

    public IngestDataCommandHandler(Context context, IPublishEndpoint publishEndpoint, DataService.DataServiceClient dataServiceClient) : base(context, publishEndpoint)
    {
        _dataServiceClient = dataServiceClient ?? throw new ArgumentNullException(nameof(dataServiceClient));
    }

    public override async Task<DomainOperationResult> Handle(IngestDataCommand command, CancellationToken cancellationToken)
    {
        var dataset = await _context.Datasets.FindAsync(command.DatasetId, cancellationToken);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        var minimumAccessLevelRequired = DatasetAccessLevel.Editor;
        if (dataset.GetUserAccessLevel(command.RequestorUserId) < minimumAccessLevelRequired)
        {
            return DomainOperationResult.Failure("User does not have the required access level.");
        }

        var rpcRequest = new IngestDataRequest
        {
            DatasetId = dataset.Id.ToString(),
            Bucket = command.Bucket,
            FileName = command.FileName,
        };

        var response = await _dataServiceClient.IngestDataAsync(rpcRequest, cancellationToken: cancellationToken);

        return response.Success
            ? DomainOperationResult.Success
            : DomainOperationResult.Failure("Failed to ingest data.");
    }
}