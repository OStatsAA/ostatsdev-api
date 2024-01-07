using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Dtos;

public record DatasetProjectLinkDto : BaseEntityDto
{
    public Guid DatasetId { get; }
    public Guid ProjectId { get; }
    public BaseDatasetDto Dataset { get; }
    public BaseProjectDto Project { get; }
    public DatasetProjectLinkDto(DatasetProjectLink link, Dataset dataset, Project project) : base(link)
    {
        DatasetId = link.DatasetId;
        ProjectId = link.ProjectId;
        Project = new BaseProjectDto(project);
        Dataset = new BaseDatasetDto(dataset);
    }
}