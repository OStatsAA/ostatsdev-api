using FluentValidation;

namespace OStats.API.Commands;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(c => c.UserAuthId).NotNull()
                                  .NotEmpty()
                                  .WithMessage("User not provided.");
    }
}