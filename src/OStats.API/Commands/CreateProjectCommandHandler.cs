using MassTransit;
using OStats.API.Commands.Common;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class CreateProjectCommandHandler : CommandHandler<CreateProjectCommand, ValueTuple<DomainOperationResult, BaseProjectDto?>>
{
    public CreateProjectCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<ValueTuple<DomainOperationResult, BaseProjectDto?>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return (DomainOperationResult.Failure("User not found."), null);
        }

        var project = new Project(user.Id, command.Title, command.Description);

        await _context.AddAsync(project);
        await SaveCommandHandlerChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseProjectDto(project));
    }
}