namespace OStats.API.Commands;

public sealed record DeleteProjectCommand
{
    public Guid RequestorId { get; }
    public Guid ProjectId { get; }

    public DeleteProjectCommand(Guid requestorId, Guid projectId)
    {
        RequestorId = requestorId;
        ProjectId = projectId;
    }
}