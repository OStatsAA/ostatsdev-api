using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class LinkProjectToDatasetCommandHandler : IRequestHandler<LinkProjectToDatasetCommand, DomainOperationResult>
{
    private readonly Context _context;

    public LinkProjectToDatasetCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DomainOperationResult> Handle(LinkProjectToDatasetCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(user => user.AuthIdentity == command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return DomainOperationResult.Failure("User not found.");
        }

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

        var result = project.LinkDataset(dataset.Id, user.Id);

        if (!result.Succeeded)
        {
            return result;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return result;
    }
}