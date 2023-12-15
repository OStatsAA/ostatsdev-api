using FluentValidation;
using OStats.API.Commands;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Validators;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    private Context _context;
    public UpdateProjectCommandValidator(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(256);

        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Project Id not provided.")
            .MustAsync(FindProjectById).WithMessage("Project not found.");

        RuleFor(c => c.UserAuthId)
            .NotEmpty().WithMessage("User not provided.")
            .MustAsync(FindUserByAuthId).WithMessage("User not found.");

        RuleFor(c => c.LastUpdatedAt)
            .NotEmpty().WithMessage("LastUpdatedAt not provided.");
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