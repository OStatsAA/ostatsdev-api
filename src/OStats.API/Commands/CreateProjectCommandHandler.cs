using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ValueTuple<DomainOperationResult, BaseProjectDto?>>
{
    private readonly Context _context;

    public CreateProjectCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ValueTuple<DomainOperationResult, BaseProjectDto?>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return (DomainOperationResult.Failure("User not found."), null);
        }

        var project = new Project(user.Id, command.Title, command.Description);

        await _context.AddAsync(project);
        await _context.SaveChangesAsync(cancellationToken);

        return (DomainOperationResult.Success, new BaseProjectDto(project));
    }

}