using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class AddUserToProjectCommandHandler : IRequestHandler<AddUserToProjectCommand, ICommandResult<bool>>
{
    private readonly Context _context;
    private readonly IValidator<AddUserToProjectCommand> _validator;

    public AddUserToProjectCommandHandler(Context context, IValidator<AddUserToProjectCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
    public async Task<ICommandResult<bool>> Handle(AddUserToProjectCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return new CommandResult<bool>(validation.Errors);
        }

        var project = _context.Projects.Local.Single(project => project.Id == request.ProjectId);
        var requestor = _context.Users.Local.Single(user => user.AuthIdentity == request.UserAuthId);
        if (!project.Roles.IsUserAtLeast(requestor.Id, AccessLevel.Administrator))
        {
            var error = new ValidationFailure("User", "Requestor cannot grant access to project.");
            return new CommandResult<bool>(error);
        }

        project.AddOrUpdateUserRole(request.UserId, request.AccessLevel);

        await _context.SaveChangesAsync(cancellationToken);
        return new CommandResult<bool>(true);
    }
}
