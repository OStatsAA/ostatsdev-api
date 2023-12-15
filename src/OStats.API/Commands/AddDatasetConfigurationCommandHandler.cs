using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class AddDatasetConfigurationCommandHandler : IRequestHandler<AddDatasetConfigurationCommand, ICommandResult<DatasetConfiguration>>
{
    private readonly Context _context;
    private readonly IValidator<AddDatasetConfigurationCommand> _validator;

    public AddDatasetConfigurationCommandHandler(Context context, IValidator<AddDatasetConfigurationCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<DatasetConfiguration>> Handle(AddDatasetConfigurationCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<DatasetConfiguration>(validation.Errors);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var project = _context.Projects.Local.Where(project => project.Id == command.ProjectId).Single();

        var minimumAccessLevelRequired = AccessLevel.Editor;
        if (!project.Roles.IsUserAtLeast(user.Id, minimumAccessLevelRequired))
        {
            var error = new ValidationFailure("UserId", $"User must be at least {minimumAccessLevelRequired}");
            return new CommandResult<DatasetConfiguration>(error);
        }

        var datasetConfiguration = new DatasetConfiguration(command.Title,
                                                            command.Source,
                                                            command.Description);
        project.AddDatasetConfiguration(datasetConfiguration);

        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<DatasetConfiguration>(datasetConfiguration);
    }
}