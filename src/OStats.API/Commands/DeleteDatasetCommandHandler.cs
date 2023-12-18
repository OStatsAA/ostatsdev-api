using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class DeleteDatasetCommandHandler : IRequestHandler<DeleteDatasetCommand, ICommandResult<bool>>
{
    private readonly Context _context;
    private readonly IValidator<DeleteDatasetCommand> _validator;

    public DeleteDatasetCommandHandler(Context context, IValidator<DeleteDatasetCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<bool>> Handle(DeleteDatasetCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<bool>(validation.Errors);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var dataset = _context.Datasets.Local.Where(dataset => dataset.Id == command.DatasetId).Single();

        if (dataset.GetUserAccess(user.Id) < DatasetAccessLevel.Owner)
        {
            var error = new ValidationFailure("User", "User cannot delete dataset.");
            return new CommandResult<bool>(error);
        }

        _context.Remove(dataset);
        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<bool>(true);
    }
}