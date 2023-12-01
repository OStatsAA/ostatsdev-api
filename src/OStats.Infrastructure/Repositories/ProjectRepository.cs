using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(Context context) : base(context)
    {
    }

    public Task<Project?> GetProjectByDatasetConfigurationIdAsync(Guid datasetConfigId)
    {
        return _context.Projects.Include(project => project.DatasetsConfigs)
                                .Where(project => project.DatasetsConfigs.Select(dc => dc.Id).Contains(datasetConfigId))
                                .SingleOrDefaultAsync();
    }
}