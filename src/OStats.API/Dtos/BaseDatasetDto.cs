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
}