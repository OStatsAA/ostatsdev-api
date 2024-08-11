using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ValueTuple<DomainOperationResult, BaseUserDto?>>
{
    private readonly Context _context;

    public CreateUserCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ValueTuple<DomainOperationResult, BaseUserDto?>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var duplicatedAuthIdentity = await _context.Users.AnyByAuthIdentityAsync(command.AuthIdentity, cancellationToken);
        if (duplicatedAuthIdentity)
        {
            return (DomainOperationResult.Failure("Cannot create user."), null);
        }

        var user = new User(command.Name, command.Email, command.AuthIdentity);
        await _context.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseUserDto(user));
    }

}