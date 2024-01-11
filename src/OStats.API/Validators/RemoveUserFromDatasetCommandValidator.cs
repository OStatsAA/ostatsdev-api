using FluentValidation;
using OStats.API.Commands;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Validators;

public class RemoveUserFromDatasetCommandValidator : AbstractValidator<RemoveUserFromDatasetCommand>
{
    private readonly Context _context;
    public RemoveUserFromDatasetCommandValidator(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.UserAuthId)
            .NotEmpty().WithMessage("Requestor not provided.")
            .MustAsync(FindUserByAuthId).WithMessage("Requestor not found.");

        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User not provided.")
            .MustAsync(FindUserById).WithMessage("User not found.");

        RuleFor(c => c.DatasetId)
            .NotNull().WithMessage("Dataset not provided.")
            .MustAsync(FindDatasetById).WithMessage("Dataset not found.");
    }

    private async Task<bool> FindUserByAuthId(string authId, CancellationToken cancellationToken)
    {
        return await _context.Users.FindByAuthIdentityAsync(authId, cancellationToken) is not null;
    }

    private async Task<bool> FindUserById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.FindAsync(id, cancellationToken) is not null;
    }

    private async Task<bool> FindDatasetById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Datasets.FindAsync(id, cancellationToken) is not null;
    }
}