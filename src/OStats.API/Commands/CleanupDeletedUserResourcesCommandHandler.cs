using MassTransit;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Common;
using OStats.Infrastructure;

namespace OStats.API.Commands;
public sealed class CleanupDeletedUserResourcesCommandHandler : CommandHandler<CleanupDeletedUserResourcesCommand, DomainOperationResult>
{
    public CleanupDeletedUserResourcesCommandHandler(Context context, IPublishEndpoint publishEndpoint) : base(context, publishEndpoint)
    {
    }

    public override async Task<DomainOperationResult> Handle(CleanupDeletedUserResourcesCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(_user => _user.Id == command.UserId, cancellationToken);
        if (user is null)
        {
            return DomainOperationResult.NoActionTaken("User is already deleted from database");
        }

        if (!user.IsDeleted)
        {
            return DomainOperationResult.Failure("User is not deleted");
        }

        List<Dataset> datasetsSingleOwnedByUser = await GetDatasetsOwnedOnlyByDeletedUserAsync(user.Id, cancellationToken);
        foreach (var dataset in datasetsSingleOwnedByUser)
        {
            dataset.Delete(user.Id);
        }

        await DeleteProjectsOwnedOnlyByDeletedUserAsync(user.Id, cancellationToken);

        _context.Remove(user);
        await SaveCommandHandlerChangesAsync(cancellationToken);
        return DomainOperationResult.Success;
    }

    private Task<List<Dataset>> GetDatasetsOwnedOnlyByDeletedUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _context.Datasets
            .Where(_ => _context.DatasetsUsersAccessLevels.Any(accessLevel => accessLevel.UserId == userId && accessLevel.AccessLevel == DatasetAccessLevel.Owner))
            .Join(_context.DatasetsUsersAccessLevels, dataset => dataset.Id, accessLevel => accessLevel.DatasetId, (dataset, _) => dataset)
            .GroupBy(dataset => dataset.Id)
            .Where(group => group.Count() == 1)
            .SelectMany(group => group.ToList())
            .ToListAsync(cancellationToken);
    }

    private async Task DeleteProjectsOwnedOnlyByDeletedUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var projectsAndRolesJoin = await _context.Projects
            .Where(_ => _context.Roles.Any(role => role.UserId == userId && role.AccessLevel == AccessLevel.Owner))
            .Join(_context.Roles, project => project.Id, role => role.ProjectId, (project, role) => new {project, role })
            .GroupBy(join => join.project.Id)
            .Where(group => group.Count() == 1)
            .SelectMany(group => group.ToList())
            .ToListAsync(cancellationToken);
        
        projectsAndRolesJoin.ForEach(join => join.project.Delete(join.role));
    }
}