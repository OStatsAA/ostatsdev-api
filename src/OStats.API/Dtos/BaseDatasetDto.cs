using System.Text.Json.Serialization;
using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.API.Dtos;

public record BaseDatasetDto : BaseEntityDto
{
    public string Title { get; }
    public string Source { get; }
    public string? Description { get; }

    public BaseDatasetDto(Dataset dataset) : base(dataset)
    {
        Title = dataset.Title;
        Source = dataset.Source;
        Description = dataset.Description;
    }

    [JsonConstructor]
    public BaseDatasetDto(Guid id, DateTime createdAt, DateTime lastUpdatedAt, string title, string source, string? description) : base(id, createdAt, lastUpdatedAt)
    {
        Title = title;
        Source = source;
        Description = description;
    }
}