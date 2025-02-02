namespace OStats.API.Commands;

public sealed record RemoveUserFromProjectCommand
{
    public Guid RequestorId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    public RemoveUserFromProjectCommand(Guid requestorId, Guid projectId, Guid userId)
    {
        RequestorId = requestorId;
        ProjectId = projectId;
        UserId = userId;
    }
}