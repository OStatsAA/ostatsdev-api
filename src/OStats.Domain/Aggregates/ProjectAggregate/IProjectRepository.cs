using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.ProjectAggregate;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetProjectByDatasetConfigurationIdAsync(Guid datasetConfigId);
}