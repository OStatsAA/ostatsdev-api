using MassTransit;
using OStats.API.Commands.Common;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class CreateDatasetCommandHandler : CommandHandler<CreateDatasetCommand, ValueTuple<DomainOperationResult, BaseDatasetDto?>>
{
    public CreateDatasetCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<ValueTuple<DomainOperationResult, BaseDatasetDto?>> Handle(CreateDatasetCommand command, CancellationToken cancellationToken)
    {
        var dataset = new Dataset(command.RequestorUserId, command.Title, command.Source, command.Description);
        await _context.AddAsync(dataset, cancellationToken);
        await SaveCommandHandlerChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseDatasetDto(dataset));
    }
}