using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Commands;

public class RemoveDatasetConfigurationCommandHandler : IRequestHandler<RemoveDatasetConfigurationCommand, ICommandResult<bool>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _usersRepository;
    private readonly IValidator<RemoveDatasetConfigurationCommand> _validator;

    public RemoveDatasetConfigurationCommandHandler(IValidator<RemoveDatasetConfigurationCommand> validator,
                                                 IProjectRepository projectRepository,
                                                 IUserRepository userRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _usersRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<bool>> Handle(RemoveDatasetConfigurationCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<bool>(validation.Errors);
        }

        var user = await _usersRepository.FindUserByAuthIdentityAsync(command.UserAuthId);
        if (user is null)
        {
            var error = new ValidationFailure("UserAuthId", "User not found.");
            return new CommandResult<bool>(error);
        }

        var project = await _projectRepository.FindByIdAsync(command.ProjectId);
        if (project is null)
        {
            var error = new ValidationFailure("ProjectId", "Project not found.");
            return new CommandResult<bool>(error);
        }

        var doesDatasetConfigExists = project.DatasetsConfigs.Any(datasetConfig => datasetConfig.Id == command.DatasetConfigurationId);
        if (!doesDatasetConfigExists)
        {
            var error = new ValidationFailure("DatasetConfigurationid", $"Dataset configuration not found.");
            return new CommandResult<bool>(error);
        }

        var minimumAccessLevelRequired = AccessLevel.Editor;
        if (!project.Roles.IsUserAtLeast(user.Id, minimumAccessLevelRequired))
        {
            var error = new ValidationFailure("UserId", $"User must be at least {minimumAccessLevelRequired}.");
            return new CommandResult<bool>(error);
        }

        project.RemoveDatasetConfiguration(command.DatasetConfigurationId);

        await _projectRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CommandResult<bool>(true);
    }
}