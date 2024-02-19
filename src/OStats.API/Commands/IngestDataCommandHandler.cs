using DataServiceGrpc;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class IngestDataCommandHandler : IRequestHandler<IngestDataCommand, ICommandResult<bool>>
{
    private readonly Context _context;
    private readonly IValidator<IngestDataCommand> _validator;
    private readonly DataService.DataServiceClient _dataServiceClient;

    public IngestDataCommandHandler(Context context, IValidator<IngestDataCommand> validator, DataService.DataServiceClient dataServiceClient)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _dataServiceClient = dataServiceClient ?? throw new ArgumentNullException(nameof(dataServiceClient));
    }

    public async Task<ICommandResult<bool>> Handle(IngestDataCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return new CommandResult<bool>(validation.Errors);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var dataset = _context.Datasets.Local.Where(dataset => dataset.Id == command.DatasetId).Single();

        var minimumAccessLevelRequired = DatasetAccessLevel.Editor;
        if (dataset.GetUserAccessLevel(user.Id) < minimumAccessLevelRequired)
        {
            var error = new ValidationFailure("UserId", $"User must be at least {minimumAccessLevelRequired}.");
            return new CommandResult<bool>(error);
        }

        var rpcRequest = new IngestDataRequest()
        {
            DatasetId = dataset.Id.ToString(),
            Bucket = command.Bucket,
            FileName = command.FileName,
        };

        var response = await _dataServiceClient.IngestDataAsync(rpcRequest);

        return new CommandResult<bool>(response.Success);
    }

}