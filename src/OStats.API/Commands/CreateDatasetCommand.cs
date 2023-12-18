using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.API.Commands;

public class CreateDatasetCommand : IRequest<ICommandResult<Dataset>>
{
    public string UserAuthId { get; set; }
    public string Title { get; set; }
    public string Source { get; set; }
    public string? Description { get; set; }
    public CreateDatasetCommand(string userAuthId, string title, string source, string? description = null)
    {
        UserAuthId = userAuthId;
        Title = title;
        Source = source;
        Description = description;
    }
}