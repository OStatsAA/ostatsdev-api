using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class ProjectQueries : IProjectQueries
{
    private readonly Context _context;
    private IQueryable<Project> _projects => _context.Projects.AsNoTracking();

    public ProjectQueries(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Project?> GetProjectByDescriptionAsync(string description)
    {
        return await _projects.Where(project => project.Description == description)
                              .SingleOrDefaultAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(Guid id)
    {
        return await _projects.Where(project => project.Id == id)
                              .Include(project => project.Roles)
                              .Include(project => project.DatasetsConfigs)
                              .SingleOrDefaultAsync();
    }

    public async Task<Project?> GetProjectByTitleAsync(string title)
    {
        return await _projects.Where(project => project.Title == title)
                              .SingleOrDefaultAsync();
    }

    public async Task<List<Project>?> GetProjectsOwnedByUserAsync(Guid userId)
    {
        Func<Role, bool> rolePredicate = role => role.UserId == userId && role.AccessLevel == AccessLevel.Owner;
        Expression<Func<Project, IEnumerable<Role>>> includeClause = project => project.Roles.Where(rolePredicate);

        return await _projects.Include(includeClause)
                              .ToListAsync();
    }

    public async Task<IReadOnlyCollection<DatasetConfiguration>?> GetDatasetsConfigurationsByProjectIdAsync(Guid projectId)
    {
        return await _projects.Where(project => project.Id == projectId)
                              .SelectMany(project => project.DatasetsConfigs)
                              .ToListAsync();
    }

    public async Task<List<ProjectUserAndRoleDto>> GetProjectUsersAndRoles(Guid projectId)
    {
        return await _context.Roles.AsNoTracking()
                                   .Where(role => role.ProjectId == projectId)
                                   .Join(_context.Users, role => role.UserId, user => user.Id, (role, user) => new ProjectUserAndRoleDto(user, role))
                                   .ToListAsync();
    }
}
