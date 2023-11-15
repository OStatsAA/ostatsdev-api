using System.ComponentModel.DataAnnotations;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Commands;

public class CreateProjectCommand : IRequest<ICommandResult<Project>>
{
    public string UserAuthId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public CreateProjectCommand(string userAuthId, string title, string? description = null)
    {
        UserAuthId = userAuthId;
        Title = title;
        Description = description;
    }
}

public struct CreateProjectCommandDto
{
    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
}