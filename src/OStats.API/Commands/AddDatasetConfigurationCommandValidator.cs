using FluentValidation;

namespace OStats.API.Commands;

public class AddDatasetConfigurationCommandValidator : AbstractValidator<AddDatasetConfigurationCommand>
{
    public AddDatasetConfigurationCommandValidator()
    {
        RuleFor(command => command.UserAuthId).NotNull()
                                              .NotEmpty()
                                              .WithMessage("User not provided.");

        RuleFor(command => command.ProjectId).NotNull()
                                             .WithMessage("Invalid project id.");
        
        RuleFor(command => command.Source).NotNull()
                                          .NotEmpty()
                                          .WithMessage("Source for dataset must be provided.");
    }
}