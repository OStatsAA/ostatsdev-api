namespace OStats.API.Commands;

public sealed record DeleteUserCommand
{
    public required string UserAuthId { get; init; }
    public required Guid UserId { get; init; }
}