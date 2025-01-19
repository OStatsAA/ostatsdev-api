using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class DeleteUserCommandHandler : CommandHandler<DeleteUserCommand, DomainOperationResult>
{
    public DeleteUserCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var toBeDeletedUser = await _context.Users.FindAsync(command.UserId, cancellationToken);
        if (toBeDeletedUser is null)
        {
            return DomainOperationResult.Failure("User not found.");
        }

        var result = toBeDeletedUser.Delete(command.RequestorUserId);
        if (!result.Succeeded)
        {
            return result;
        }
        
        await SaveCommandHandlerChangesAsync(cancellationToken);
        return DomainOperationResult.Success;
    }
}