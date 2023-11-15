using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(Context context) : base(context)
    {
    }
}