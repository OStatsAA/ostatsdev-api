using DataServiceGrpc;
using MediatR;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class IngestDataCommandHandler : IRequestHandler<IngestDataCommand, DomainOperationResult>
{
    private readonly Context _context;
    private readonly DataService.DataServiceClient _dataServiceClient;

    public IngestDataCommandHandler(Context context, DataService.DataServiceClient dataServiceClient)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dataServiceClient = dataServiceClient ?? throw new ArgumentNullException(nameof(dataServiceClient));
    }

    public async Task<DomainOperationResult> Handle(IngestDataCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return DomainOperationResult.Failure("User not found.");
        }

        var dataset = await _context.Datasets.FindAsync(command.DatasetId, cancellationToken);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        var minimumAccessLevelRequired = DatasetAccessLevel.Editor;
        if (dataset.GetUserAccessLevel(user.Id) < minimumAccessLevelRequired)
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