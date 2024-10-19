namespace OStats.API.Commands;

public sealed record DeleteProjectCommand
{
    public string UserAuthId { get; }
    public Guid ProjectId { get; }

    public DeleteProjectCommand(string userAuthId, Guid projectId)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
    }
}