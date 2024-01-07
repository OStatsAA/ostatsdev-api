using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Dtos;

public record BaseProjectDto : BaseEntityDto
{
    public string Title { get; }
    public string? Description { get; }

    public BaseProjectDto(Project project) : base(project)
    {
        Title = project.Title;
        Description = project.Description;
    }
}