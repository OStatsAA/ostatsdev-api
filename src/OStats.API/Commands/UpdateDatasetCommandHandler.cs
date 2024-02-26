using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class UpdateDatasetCommandHandler : IRequestHandler<UpdateDatasetCommand, ValueTuple<DomainOperationResult, BaseDatasetDto?>>
{
    private readonly Context _context;

    public UpdateDatasetCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ValueTuple<DomainOperationResult, BaseDatasetDto?>> Handle(UpdateDatasetCommand command, CancellationToken cancellationToken)
    {
        var dataset = await _context.Datasets.FindAsync(command.Id, cancellationToken);
        if (dataset is null)
        {
            return (DomainOperationResult.Failure("Dataset not found."), null);
        }

        if (dataset.LastUpdatedAt > command.LastUpdatedAt)
        {
            return (DomainOperationResult.Failure("Project has changed since command was submited."), null);
        }

        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return (DomainOperationResult.Failure("User not found."), null);
        }


        var minimumAccessLevelRequired = DatasetAccessLevel.Editor;
        if (dataset.GetUserAccessLevel(user.Id) < minimumAccessLevelRequired)
        {
            return (DomainOperationResult.Failure("User does not have the required access level."), null);
        }

        dataset.Title = command.Title;
        dataset.Source = command.Source;
        dataset.Description = command.Description;

        await _context.SaveChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseDatasetDto(dataset));
    }

}