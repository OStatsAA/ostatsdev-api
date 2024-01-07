using MediatR;
using OStats.API.Common;
using OStats.API.Dtos;

namespace OStats.API.Commands;

public class CreateUserCommand : IRequest<ICommandResult<BaseUserDto>>
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