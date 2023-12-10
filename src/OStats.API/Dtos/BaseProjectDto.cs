using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Dtos;

public class BaseProjectDto
{
    public Guid Id { get; }
    public string Title { get; }
    public string? Description { get; }
    public IReadOnlyCollection<DatasetConfiguration> DatasetsConfigs { get; }

    public BaseProjectDto(Project project)
    {
        Id = project.Id;
        Title = project.Title;
        Description = project.Description;
        DatasetsConfigs = project.DatasetsConfigs;
    }
}