using FluentValidation;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ICommandResult<User>>
{
    private readonly Context _context;
    private readonly IValidator<CreateUserCommand> _validator;

    public CreateUserCommandHandler(Context context, IValidator<CreateUserCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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

        await _context.AddAsync(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<User>(user);
    }

}