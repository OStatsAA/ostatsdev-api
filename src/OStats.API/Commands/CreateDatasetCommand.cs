using MediatR;
using OStats.API.Dtos;
using OStats.Domain.Common;

namespace OStats.API.Commands;

public class CreateDatasetCommand : IRequest<ValueTuple<DomainOperationResult, BaseDatasetDto?>>
{
    public string UserAuthId { get; set; }
    public string Title { get; set; }
    public string Source { get; set; }
    public string Description { get; set; }

    public CreateDatasetCommand(string userAuthId, string title, string source, string? description)
    {
        UserAuthId = userAuthId;
        Title = title;
        Source = source;
        Description = description ?? string.Empty;
    }
}