using FluentValidation;
using FluentValidation.Validators;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private IUserRepository _userRepository;
    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(c => c.Email).NotNull()
                             .NotEmpty()
                             .WithMessage("User email address not provided.");

        RuleFor(c => c.Email).EmailAddress(EmailValidationMode.AspNetCoreCompatible);

        RuleFor(c => c.Name).NotNull()
                            .NotEmpty()
                            .WithMessage("User name not provided.");

        RuleFor(c => c.AuthIdentity).NotNull()
                                    .NotEmpty()
                                    .WithMessage("User AuthIdentity not provided.");

        RuleFor(c => c.AuthIdentity)
            .MustAsync(async (authId, cancellation) => !await _userRepository.ExistsByAuthIdentityAsync(authId))
            .WithMessage("User cannot be registered.");
    }
}