using MassTransit;
using OStats.API.Commands.Common;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class CreateUserCommandHandler : CommandHandler<CreateUserCommand, ValueTuple<DomainOperationResult, BaseUserDto?>>
{
    public CreateUserCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<ValueTuple<DomainOperationResult, BaseUserDto?>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var duplicatedAuthIdentity = await _context.Users.AnyByAuthIdentityAsync(command.AuthIdentity, cancellationToken);
        if (duplicatedAuthIdentity)
        {
            return (DomainOperationResult.Failure("Cannot create user."), null);
        }

        var user = new User(command.Name, command.Email, command.AuthIdentity);
        await _context.AddAsync(user, cancellationToken);
        await SaveCommandHandlerChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseUserDto(user));
    }
}