namespace OStats.API.Commands;

public sealed record DeleteProjectCommand
{
    public Guid RequestorUserId { get; }
    public Guid ProjectId { get; }

    public DeleteProjectCommand(Guid requestorUserId, Guid projectId)
    {
        RequestorUserId = requestorUserId;
        ProjectId = projectId;
    }
}