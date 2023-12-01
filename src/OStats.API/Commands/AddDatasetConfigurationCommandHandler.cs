using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Commands;

public class AddDatasetConfigurationCommandHandler : IRequestHandler<AddDatasetConfigurationCommand, ICommandResult<DatasetConfiguration>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _usersRepository;
    private readonly IValidator<AddDatasetConfigurationCommand> _validator;

    public AddDatasetConfigurationCommandHandler(IValidator<AddDatasetConfigurationCommand> validator,
                                                 IProjectRepository projectRepository,
                                                 IUserRepository userRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _usersRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<DatasetConfiguration>> Handle(AddDatasetConfigurationCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<DatasetConfiguration>(validation.Errors);
        }

        var user = await _usersRepository.FindUserByAuthIdentityAsync(command.UserAuthId);
        if (user is null)
        {
            var error = new ValidationFailure("UserAuthId", "User not found.");
            return new CommandResult<DatasetConfiguration>(error);
        }

        var project = await _projectRepository.FindByIdAsync(command.ProjectId);
        if (project is null)
        {
            var error = new ValidationFailure("ProjectId", "Project not found.");
            return new CommandResult<DatasetConfiguration>(error);
        }

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

        await _projectRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CommandResult<DatasetConfiguration>(datasetConfiguration);
    }
}