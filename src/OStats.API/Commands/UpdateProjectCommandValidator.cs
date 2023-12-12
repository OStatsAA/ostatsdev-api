using FluentValidation;

namespace OStats.API.Commands;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(c => c.Id).NotNull()
                          .NotEmpty()
                          .WithMessage("Project Id not provided.");
        RuleFor(c => c.UserAuthId).NotNull()
                                  .NotEmpty()
                                  .WithMessage("User not provided.");
    }
}