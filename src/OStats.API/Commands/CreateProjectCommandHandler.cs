using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Commands;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ICommandResult<Project>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _usersRepository;
    private readonly IValidator<CreateProjectCommand> _validator;

    public CreateProjectCommandHandler(IValidator<CreateProjectCommand> validator,
                                       IProjectRepository projectRepository,
                                       IUserRepository userRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _usersRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<Project>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<Project>(validation.Errors);
        }

        var user = await _usersRepository.FindUserByAuthIdentityAsync(command.UserAuthId);
        if (user == null)
        {
            var error = new ValidationFailure("UserAuthId", "User not found.");
            return new CommandResult<Project>(error);
        }

        var project = new Project(user.Id, command.Title, command.Description);
        _projectRepository.Add(project);
        await _projectRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CommandResult<Project>(project);
    }

}