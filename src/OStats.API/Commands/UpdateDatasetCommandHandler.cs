using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class UpdateDatasetCommandHandler : IRequestHandler<UpdateDatasetCommand, ICommandResult<Dataset>>
{
    private readonly Context _context;
    private readonly IValidator<UpdateDatasetCommand> _validator;

    public UpdateDatasetCommandHandler(Context context, IValidator<UpdateDatasetCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<Dataset>> Handle(UpdateDatasetCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<Dataset>(validation.Errors);
        }

        var dataset = _context.Datasets.Local.Where(dataset => dataset.Id == command.Id).Single();
        if (dataset.LastUpdatedAt != command.LastUpdatedAt)
        {
            var error = new ValidationFailure("LastUpdatedAt", "Dataset has changed since command was submited.");
            return new CommandResult<Dataset>(error);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var minimumAccessLevelRequired = DatasetAccessLevel.Editor;
        if (dataset.GetUserAccess(user.Id) < minimumAccessLevelRequired)
        {
            var error = new ValidationFailure("UserId", $"User must be at least {minimumAccessLevelRequired}.");
            return new CommandResult<Dataset>(error);
        }

        dataset.Title = command.Title;
        dataset.Source = command.Source;
        dataset.Description = command.Description;

        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<Dataset>(dataset);
    }

}