namespace OStats.API.Commands;

public sealed class CreateUserCommand
{
    public string Name { get; }
    public string Email { get; }
    public string AuthIdentity { get; }
    public CreateUserCommand(string name, string email, string authId)
    {
        Name = name;
        Email = email;
        AuthIdentity = authId;
    }
}