namespace OStats.API.Commands;

public record CreateDatasetCommand
{
    public Guid RequestorUserId { get; set; }
    public string Title { get; set; }
    public string Source { get; set; }
    public string Description { get; set; }

    public CreateDatasetCommand(Guid requestorUserId, string title, string source, string? description)
    {
        RequestorUserId = requestorUserId;
        Title = title;
        Source = source;
        Description = description ?? string.Empty;
    }
}