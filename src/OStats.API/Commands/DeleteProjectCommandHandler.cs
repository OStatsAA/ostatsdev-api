using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Commands;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, ICommandResult<bool>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _usersRepository;
    private readonly IValidator<DeleteProjectCommand> _validator;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository,
                                       IUserRepository userRepository,
                                       IValidator<DeleteProjectCommand> validator)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _usersRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<bool>> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
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

        var isUserOwner = project.Roles.IsUser(user.Id, AccessLevel.Owner);
        if (!isUserOwner)
        {
            var error = new ValidationFailure("UserAuthId", "User cannot delete project.");
            return new CommandResult<bool>(error);
        }

        _projectRepository.Delete(project);
        await _projectRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return new CommandResult<bool>(true);
    }
}