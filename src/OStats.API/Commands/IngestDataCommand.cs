namespace OStats.API.Commands;

public sealed record IngestDataCommand
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