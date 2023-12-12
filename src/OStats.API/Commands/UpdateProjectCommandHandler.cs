using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Commands;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ICommandResult<Project>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _usersRepository;
    private readonly IValidator<UpdateProjectCommand> _validator;

    public UpdateProjectCommandHandler(IValidator<UpdateProjectCommand> validator,
                                       IProjectRepository projectRepository,
                                       IUserRepository userRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _usersRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<Project>> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<Project>(validation.Errors);
        }

        var project = await _projectRepository.FindByIdAsync(command.Id);
        if (project is null)
        {
            var error = new ValidationFailure("Id", "Project not found.");
            return new CommandResult<Project>(error);
        }

        if (project.LastUpdatedAt != command.LastUpdatedAt)
        {
            var error = new ValidationFailure("LastUpdatedAt", "Project has changed since command was submited.");
            return new CommandResult<Project>(error);
        }

        var user = await _usersRepository.FindUserByAuthIdentityAsync(command.UserAuthId);
        if (user is null)
        {
            var error = new ValidationFailure("UserAuthId", "User not found.");
            return new CommandResult<Project>(error);
        }

        project.Title = command.Title;
        project.Description = command.Description;

        await _projectRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CommandResult<Project>(project);
    }

}