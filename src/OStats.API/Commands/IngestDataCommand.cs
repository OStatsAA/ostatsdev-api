using MediatR;
using OStats.API.Common;

namespace OStats.API.Commands;

public class IngestDataCommand : IRequest<ICommandResult<bool>>
{
    public Guid DatasetId { get; }
    public string UserAuthId { get; }
    public string Bucket { get; }
    public string FileName { get; }

    public IngestDataCommand(string userAuthId, Guid datasetId, string bucket, string fileName)
    {
        UserAuthId = userAuthId;
        DatasetId = datasetId;
        Bucket = bucket;
        FileName = fileName;
    }
}