namespace OStats.API.Commands;

public record CreateProjectCommand
{
    public Guid RequestorUserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public CreateProjectCommand(Guid requestorUserId, string title, string? description)
    {
        RequestorUserId = requestorUserId;
        Title = title;
        Description = description ?? string.Empty;
    }
}