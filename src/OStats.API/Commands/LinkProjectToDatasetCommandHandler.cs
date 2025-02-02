using MassTransit;
using OStats.API.Commands.Common;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public sealed class LinkProjectToDatasetCommandHandler : CommandHandler<LinkProjectToDatasetCommand, DomainOperationResult>
{
    public LinkProjectToDatasetCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(LinkProjectToDatasetCommand command, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FindAsync(command.ProjectId, cancellationToken);
        if (project is null)
        {
            return DomainOperationResult.Failure("Project not found.");
        }

        var dataset = await _context.Datasets.FindAsync(command.DatasetId, cancellationToken);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        var requestorRole = await _context.Roles
            .FindByProjectIdAndUserIdAsync(command.ProjectId, command.RequestorUserId, cancellationToken);
        if (requestorRole is null)
        {
            return DomainOperationResult.InvalidUserRole();
        }

        var result = project.LinkDataset(dataset.Id, requestorRole);

        if (!result.Succeeded)
        {
            return result;
        }

        await SaveCommandHandlerChangesAsync(cancellationToken);

        return result;
    }
}