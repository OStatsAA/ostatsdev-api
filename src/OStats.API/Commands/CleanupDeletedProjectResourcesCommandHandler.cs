using MassTransit;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;
public sealed class CleanupDeletedProjectResourcesCommandHandler : CommandHandler<CleanupDeletedProjectResourcesCommand, DomainOperationResult>
{
    public CleanupDeletedProjectResourcesCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(CleanupDeletedProjectResourcesCommand command, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.IgnoreQueryFilters().SingleOrDefaultAsync(p => p.Id == command.ProjectId, cancellationToken);

        if (project is null)
        {
            return DomainOperationResult.NoActionTaken("Project is already deleted from database");
        }

        if (!project.IsDeleted)
        {
            return DomainOperationResult.Failure("Project is not deleted");
        }

        _context.Remove(project);

        await SaveCommandHandlerChangesAsync(cancellationToken);
        return DomainOperationResult.Success;
    }
}