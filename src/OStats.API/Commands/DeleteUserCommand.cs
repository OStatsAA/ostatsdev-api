namespace OStats.API.Commands;

public sealed record DeleteUserCommand
{
    public required Guid RequestorUserId { get; init; }
    public required Guid UserId { get; init; }
}