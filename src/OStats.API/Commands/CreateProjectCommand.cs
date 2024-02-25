using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public class CreateProjectCommand : IRequest<ValueTuple<DomainOperationResult, BaseProjectDto?>>
{
    public string UserAuthId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }

    public CreateProjectCommand(string userAuthId, string title)
    {
        UserAuthId = userAuthId;
        Title = title;
    }
    public CreateProjectCommand(string userAuthId, string title, string description)
    {
        UserAuthId = userAuthId;
        Title = title;
        Description = description;
    }
}