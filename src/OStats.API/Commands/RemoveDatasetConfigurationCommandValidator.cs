using FluentValidation;

namespace OStats.API.Commands;

public class RemoveDatasetConfigurationCommandValidator : AbstractValidator<RemoveDatasetConfigurationCommand>
{
    public RemoveDatasetConfigurationCommandValidator()
    {
        RuleFor(command => command.UserAuthId).NotNull()
                                              .NotEmpty()
                                              .WithMessage("User not provided.");

        RuleFor(command => command.ProjectId).NotNull()
                                             .WithMessage("Project not provided.");

        RuleFor(command => command.DatasetConfigurationId).NotNull()
                                                          .WithMessage("Dataset configuration not provided.");
    }
}