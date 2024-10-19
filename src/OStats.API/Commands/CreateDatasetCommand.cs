namespace OStats.API.Commands;

public record CreateDatasetCommand
{
    public string UserAuthId { get; set; }
    public string Title { get; set; }
    public string Source { get; set; }
    public string Description { get; set; }

    public CreateDatasetCommand(string userAuthId, string title, string source, string? description)
    {
        UserAuthId = userAuthId;
        Title = title;
        Source = source;
        Description = description ?? string.Empty;
    }
}