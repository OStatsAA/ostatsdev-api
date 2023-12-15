using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, ICommandResult<bool>>
{
    private readonly Context _context;
    private readonly IValidator<DeleteProjectCommand> _validator;

    public DeleteProjectCommandHandler(Context context, IValidator<DeleteProjectCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<bool>> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<bool>(validation.Errors);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var project = _context.Projects.Local.Where(project => project.Id == command.ProjectId).Single();

        var isUserOwner = project.Roles.IsUser(user.Id, AccessLevel.Owner);
        if (!isUserOwner)
        {
            var error = new ValidationFailure("UserAuthId", "User cannot delete project.");
            return new CommandResult<bool>(error);
        }

        _context.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<bool>(true);
    }
}