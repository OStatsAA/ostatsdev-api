namespace OStats.API.Commands;

public sealed record IngestDataCommand
{
    public Guid DatasetId { get; }
    public Guid RequestorUserId { get; }
    public string Bucket { get; }
    public string FileName { get; }

    public IngestDataCommand(Guid requestorUserId, Guid datasetId, string bucket, string fileName)
    {
        RequestorUserId = requestorUserId;
        DatasetId = datasetId;
        Bucket = bucket;
        FileName = fileName;
    }
}