using System.Text.Json.Serialization;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Dtos;

public record BaseProjectDto : BaseEntityDto
{
    public string Title { get; init;}
    public string? Description { get; init; }

    public BaseProjectDto(Project project) : base(project)
    {
        Title = project.Title;
        Description = project.Description;
    }

    [JsonConstructor]
    public BaseProjectDto(Guid id, DateTime createdAt, DateTime lastUpdatedAt, string title, string? description) : base(id, createdAt, lastUpdatedAt)
    {
        Title = title;
        Description = description;
    }
}