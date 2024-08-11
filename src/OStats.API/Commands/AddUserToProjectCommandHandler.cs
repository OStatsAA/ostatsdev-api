using MediatR;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class AddUserToProjectCommandHandler : IRequestHandler<AddUserToProjectCommand, DomainOperationResult>
{
    private readonly Context _context;

    public AddUserToProjectCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DomainOperationResult> Handle(AddUserToProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FindAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return DomainOperationResult.Failure("Project not found");
        }

        var requestor = await _context.Users.FindByAuthIdentityAsync(request.UserAuthId, cancellationToken);
        if (requestor is null)
        {
            return DomainOperationResult.Failure("Requestor not found");
        }

        var result = project.AddOrUpdateUserRole(request.UserId, request.AccessLevel, requestor.Id);
        if (!result.Succeeded)
        {
            return result;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return result;
    }
}
