using FluentValidation;
using OStats.API.Commands;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Validators;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    private Context _context;
    public DeleteProjectCommandValidator(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(command => command.UserAuthId)
            .NotEmpty().WithMessage("User not provided.")
            .MustAsync(FindUserByAuthId).WithMessage("User not found.");

        RuleFor(command => command.ProjectId)
            .NotNull().WithMessage("Project not provided.")
            .MustAsync(FindProjectById).WithMessage("Project not found.");
    }

    private async Task<bool> FindUserByAuthId(string authId, CancellationToken cancellationToken)
    {
        return await _context.Users.FindByAuthIdentityAsync(authId, cancellationToken) is not null;
    }

    private async Task<bool> FindProjectById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Projects.FindAsync(id, cancellationToken) is not null;
    }
}