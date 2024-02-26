using MediatR;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class RemoveUserFromProjectCommandHandler : IRequestHandler<RemoveUserFromProjectCommand, DomainOperationResult>
{
    private readonly Context _context;

    public RemoveUserFromProjectCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DomainOperationResult> Handle(RemoveUserFromProjectCommand request, CancellationToken cancellationToken)
    {

        var requestor = await _context.Users.FindByAuthIdentityAsync(request.UserAuthId, cancellationToken);
        if (requestor is null)
        {
            return DomainOperationResult.Failure("Requestor not found.");
        }

        var project = await _context.Projects.FindAsync(request.ProjectId);
        if (project is null)
        {
            return DomainOperationResult.Failure("Project not found.");
        }

        var result = project.RemoveUserRole(request.UserId, requestor.Id);

        if (!result.Succeeded)
        {
            return result;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return result;
    }
}