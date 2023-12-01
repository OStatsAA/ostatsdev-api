using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Queries;

public interface IProjectQueries
{
    Task<Project?> GetProjectByIdAsync(Guid id);
    Task<Project?> GetProjectByTitleAsync(string title);
    Task<Project?> GetProjectByDescriptionAsync(string description);
    Task<List<Project>?> GetProjectsOwnedByUserAsync(Guid userId);
    Task<IReadOnlyCollection<DatasetConfiguration>?> GetDatasetsConfigurationsByProjectIdAsync(Guid projectId);
}