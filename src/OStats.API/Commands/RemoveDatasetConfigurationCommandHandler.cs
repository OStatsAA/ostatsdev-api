using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class RemoveDatasetConfigurationCommandHandler : IRequestHandler<RemoveDatasetConfigurationCommand, ICommandResult<bool>>
{
    private readonly Context _context;
    private readonly IValidator<RemoveDatasetConfigurationCommand> _validator;

    public RemoveDatasetConfigurationCommandHandler(Context context, IValidator<RemoveDatasetConfigurationCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<bool>> Handle(RemoveDatasetConfigurationCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<bool>(validation.Errors);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var project = _context.Projects.Local.Where(project => project.Id == command.ProjectId).Single();

        var doesDatasetConfigExists = project.DatasetsConfigs.Any(datasetConfig => datasetConfig.Id == command.DatasetConfigurationId);
        if (!doesDatasetConfigExists)
        {
            var error = new ValidationFailure("DatasetConfigurationid", $"Dataset configuration not linked to project.");
            return new CommandResult<bool>(error);
        }

        var minimumAccessLevelRequired = AccessLevel.Editor;
        if (!project.Roles.IsUserAtLeast(user.Id, minimumAccessLevelRequired))
        {
            var error = new ValidationFailure("UserId", $"User must be at least {minimumAccessLevelRequired}.");
            return new CommandResult<bool>(error);
        }

        project.RemoveDatasetConfiguration(command.DatasetConfigurationId);

        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<bool>(true);
    }
}