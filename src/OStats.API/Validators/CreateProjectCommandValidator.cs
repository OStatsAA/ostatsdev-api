using FluentValidation;
using OStats.API.Commands;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Validators;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    private readonly Context _context;
    public CreateProjectCommandValidator(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(256);

        RuleFor(c => c.UserAuthId)
            .NotEmpty().WithMessage("User not provided.")
            .MustAsync(FindUserByAuthId).WithMessage("User not found.");
    }

    private async Task<bool> FindUserByAuthId(string authId, CancellationToken cancellationToken)
    {
        return await _context.Users.FindByAuthIdentityAsync(authId, cancellationToken) is not null;
    }
}