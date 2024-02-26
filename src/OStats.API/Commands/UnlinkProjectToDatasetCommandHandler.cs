using MediatR;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class UnlinkProjectToDatasetCommandHandler : IRequestHandler<UnlinkProjectToDatasetCommand, DomainOperationResult>
{
    private readonly Context _context;

    public UnlinkProjectToDatasetCommandHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<DomainOperationResult> Handle(UnlinkProjectToDatasetCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindByAuthIdentityAsync(command.UserAuthId, cancellationToken);
        if (user is null)
        {
            return DomainOperationResult.Failure("User not found.");
        }

        var project = await _context.Projects.FindAsync(command.ProjectId);
        if (project is null)
        {
            return DomainOperationResult.Failure("Project not found.");
        }

        var dataset = await _context.Datasets.FindAsync(command.DatasetId);
        if (dataset is null)
        {
            return DomainOperationResult.Failure("Dataset not found.");
        }

        var result = project.UnlinkDataset(dataset.Id, user.Id);
        if (!result.Succeeded)
        {
            return result;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return result;
    }
}