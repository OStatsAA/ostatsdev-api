using System.ComponentModel.DataAnnotations;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Commands;

public class AddDatasetConfigurationCommand : IRequest<ICommandResult<DatasetConfiguration>>
{
    public string UserAuthId { get; }
    public Guid ProjectId { get; }
    public string Title { get; }
    public string Source { get; }
    public string? Description { get; }

    public AddDatasetConfigurationCommand(string userAuthId,
                                          Guid projectId,
                                          string title,
                                          string source,
                                          string? description = null)
    {
        UserAuthId = userAuthId;
        ProjectId = projectId;
        Title = title;
        Source = source;
        Description = description;
    }
}

public class AddDatasetConfigurationCommandDto
{
    [Required]
    public string Title { get; }
    [Required]
    public string Source { get; }
    public string? Description { get; }

    public AddDatasetConfigurationCommandDto(string title,
                                             string source,
                                             string? description = null)
    {
        Title = title;
        Source = source;
        Description = description;
    }
}
