using FluentValidation;
using OStats.API.Commands;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Validators;

public class LinkProjectToDatasetCommandValidator : AbstractValidator<LinkProjectToDatasetCommand>
{
    private readonly Context _context;
    public LinkProjectToDatasetCommandValidator(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(command => command.UserAuthId)
            .NotEmpty().WithMessage("User not provided.")
            .MustAsync(FindUserByAuthId).WithMessage("User not found.");

        RuleFor(command => command.ProjectId)
            .NotEmpty().WithMessage("Invalid project id.")
            .MustAsync(FindProjectById).WithMessage("Project not found.");

        RuleFor(command => command.DatasetId)
            .NotEmpty().WithMessage("Dataset id must be provided.")
            .MustAsync(FindDatasetById).WithMessage("Dataset not found.");
    }

    private async Task<bool> FindUserByAuthId(string authId, CancellationToken cancellationToken)
    {
        return await _context.Users.FindByAuthIdentityAsync(authId, cancellationToken) is not null;
    }

    private async Task<bool> FindDatasetById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Datasets.FindAsync(id, cancellationToken) is not null;
    }

    private async Task<bool> FindProjectById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Projects.FindAsync(id, cancellationToken) is not null;
    }
}