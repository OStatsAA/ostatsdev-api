using FluentValidation;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ICommandResult<User>>
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserCommand> _validator;

    public CreateUserCommandHandler(IValidator<CreateUserCommand> validator,
                                    IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<User>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<User>(validation.Errors);
        }

        var user = new User(command.Name, command.Email, command.AuthIdentity);
        _userRepository.Add(user);
        await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return new CommandResult<User>(user);
    }

}