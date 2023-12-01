using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.ProjectAggregate;

public class DatasetConfiguration : Entity
{
    public Guid ProjectId { get; private set; }
    public string Title { get; }
    public string Source { get; }
    public string? Description { get; }

    public DatasetConfiguration(string title, string source, string? description = null)
    {
        Title = title;
        Source = source;
        Description = description;
    }
}