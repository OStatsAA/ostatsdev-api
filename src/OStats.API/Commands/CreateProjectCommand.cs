namespace OStats.API.Commands;

public record CreateProjectCommand
{
    public string UserAuthId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public CreateProjectCommand(string userAuthId, string title, string? description)
    {
        UserAuthId = userAuthId;
        Title = title;
        Description = description ?? string.Empty;
    }
}