using FluentValidation;
using OStats.API.Commands;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Validators;

public class AddUserToProjectCommandValidator : AbstractValidator<AddUserToProjectCommand>
{
    private readonly Context _context;
    public AddUserToProjectCommandValidator(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.UserAuthId)
            .NotEmpty().WithMessage("Requestor not provided.")
            .MustAsync(FindUserByAuthId).WithMessage("Requestor not found.");

        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User not provided.")
            .MustAsync(FindUserById).WithMessage("User not found.");

        RuleFor(c => c.ProjectId)
            .NotNull().WithMessage("Project not provided.")
            .MustAsync(FindProjectById).WithMessage("Project not found.");
    }

    private async Task<bool> FindUserByAuthId(string authId, CancellationToken cancellationToken)
    {
        return await _context.Users.FindByAuthIdentityAsync(authId, cancellationToken) is not null;
    }

    private async Task<bool> FindUserById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.FindAsync(id, cancellationToken) is not null;
    }

    private async Task<bool> FindProjectById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Projects.FindAsync(id, cancellationToken) is not null;
    }
}