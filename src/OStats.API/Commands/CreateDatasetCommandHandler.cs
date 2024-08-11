using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class CreateDatasetCommandHandler : IRequestHandler<CreateDatasetCommand, ValueTuple<DomainOperationResult, BaseDatasetDto?>>
{
    private readonly Context _context;
    public CreateDatasetCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ValueTuple<DomainOperationResult, BaseDatasetDto?>> Handle(CreateDatasetCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return (DomainOperationResult.Failure("User not found."), null);
        }

        var dataset = new Dataset(user.Id, command.Title, command.Source, command.Description);
        await _context.AddAsync(dataset, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseDatasetDto(dataset));
    }

}