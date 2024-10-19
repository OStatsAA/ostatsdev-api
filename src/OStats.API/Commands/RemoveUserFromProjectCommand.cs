namespace OStats.API.Commands;

public sealed record RemoveUserFromProjectCommand
{
    public string UserAuthId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    public RemoveUserFromProjectCommand(string userAuthId, Guid projectId, Guid userId)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
        UserId = userId;
    }
}