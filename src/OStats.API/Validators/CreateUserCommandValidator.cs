using FluentValidation;
using FluentValidation.Validators;
using OStats.API.Commands;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private Context _context;
    public CreateUserCommandValidator(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("User email address not provided.")
            .EmailAddress(EmailValidationMode.AspNetCoreCompatible);

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("User name not provided.");

        RuleFor(c => c.AuthIdentity)
            .NotEmpty().WithMessage("User authentication not provided.")
            .MustAsync(NotAlreadyExistByAuthId).WithMessage("User not found.");
    }

    private async Task<bool> NotAlreadyExistByAuthId(string authId, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyByAuthIdentityAsync(authId, cancellationToken);
    }
}