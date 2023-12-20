using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.API.Dtos;

public class BaseDatasetDto
{
    public Guid Id { get; }
    public string Title { get; }
    public string Source { get; }
    public string? Description { get; }

    public BaseDatasetDto(Dataset dataset)
    {
        Id = dataset.Id;
        Title = dataset.Title;
        Source = dataset.Source;
        Description = dataset.Description;
    }
}