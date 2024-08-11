using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public sealed class CreateUserCommand : IRequest<ValueTuple<DomainOperationResult, BaseUserDto?>>
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