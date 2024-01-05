using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Dtos;

public record DatasetProjectLinkDto : BaseEntityDto
{
    public Guid DatasetId { get; }
    public BaseProjectDto Project { get; }
    public DatasetProjectLinkDto(DatasetProjectLink link, Project project) : base(link)
    {
        DatasetId = link.DatasetId;
        Project = new BaseProjectDto(project);
    }
}