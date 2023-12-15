using FluentValidation;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ICommandResult<Project>>
{
    private readonly Context _context;
    private readonly IValidator<CreateProjectCommand> _validator;

    public CreateProjectCommandHandler(Context context, IValidator<CreateProjectCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<Project>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return new CommandResult<Project>(validation.Errors);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var project = new Project(user.Id, command.Title, command.Description);

        await _context.AddAsync(project);
        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<Project>(project);
    }

}