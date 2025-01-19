namespace OStats.API.Commands;

public sealed record RemoveUserFromProjectCommand
{
    public Guid RequestorUserId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    public RemoveUserFromProjectCommand(Guid requestorUserId, Guid projectId, Guid userId)
    {
        RequestorUserId = requestorUserId;
        ProjectId = projectId;
        UserId = userId;
    }
}