using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Dtos;

public record ProjectDto : BaseProjectDto
{
    public IReadOnlyCollection<DatasetProjectLinkDto> LinkedDatasets { get; }
    public ProjectDto(Project project, List<Dataset> linkedDatasets) : base(project)
    {
        LinkedDatasets = project.LinkedDatasets
            .Select(link => new DatasetProjectLinkDto(
                link,
                linkedDatasets.Where(dataset => dataset.Id == link.DatasetId).Single(),
                project))
            .ToList();
    }
}