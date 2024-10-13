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
        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return (DomainOperationResult.Failure("User not found."), null);
        }

        var dataset = new Dataset(user.Id, command.Title, command.Source, command.Description);
        await _context.AddAsync(dataset, cancellationToken);
        await SaveCommandHandlerChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseDatasetDto(dataset));
    }
}