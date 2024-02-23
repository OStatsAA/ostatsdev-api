using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Dtos;

public record DatasetProjectLinkDto : BaseEntityDto
{
    public BaseDatasetDto Dataset { get; init; }
    public BaseProjectDto Project { get; init; }
    public DatasetProjectLinkDto(DatasetProjectLink link, Dataset dataset, Project project) : base(link)
    {
        Project = new BaseProjectDto(project);
        Dataset = new BaseDatasetDto(dataset);
    }
}