using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ICommandResult<Project>>
{
    private readonly Context _context;
    private readonly IValidator<UpdateProjectCommand> _validator;

    public UpdateProjectCommandHandler(Context context, IValidator<UpdateProjectCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<Project>> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return new CommandResult<Project>(validation.Errors);
        }

        var project = _context.Projects.Local.Where(project => project.Id == command.Id).Single();
        if (project.LastUpdatedAt != command.LastUpdatedAt)
        {
            var error = new ValidationFailure("LastUpdatedAt", "Project has changed since command was submited.");
            return new CommandResult<Project>(error);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var minimumAccessLevelRequired = AccessLevel.Editor;
        if (!project.Roles.IsUserAtLeast(user.Id, minimumAccessLevelRequired))
        {
            var error = new ValidationFailure("UserId", $"User must be at least {minimumAccessLevelRequired}.");
            return new CommandResult<Project>(error);
        }

        project.Title = command.Title;
        project.Description = command.Description;

        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<Project>(project);
    }

}