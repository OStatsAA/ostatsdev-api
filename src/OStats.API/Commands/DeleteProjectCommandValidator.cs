using FluentValidation;

namespace OStats.API.Commands;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(command => command.UserAuthId).NotNull()
                                              .NotEmpty()
                                              .WithMessage("User not provided.");

        RuleFor(command => command.ProjectId).NotNull()
                                             .WithMessage("Project not provided.");
    }
}