namespace OStats.Domain.Common;

public record AggregateHistoryEntry
{
    private Guid _id;
    public virtual Guid Id
    {
        get
        {
            return _id;
        }
        protected set
        {
            _id = value;
        }
    }
    public Guid AggregateId { get; init; }
    public string AggregateType { get; init; }
    public Guid UserId { get; init; }
    public string EventType { get; init; }
    public string EventData { get; init; }
    public string EventDescription { get; init;}
    public DateTime CreatedAt { get; init; }

    public AggregateHistoryEntry(Guid aggregateId, string aggregateType, Guid userId, string eventType, string eventData, string eventDescription, DateTime createdAt)
    {
        AggregateId = aggregateId;
        AggregateType = aggregateType;
        UserId = userId;
        EventType = eventType;
        EventData = eventData;
        EventDescription = eventDescription;
        CreatedAt = createdAt;
    }
}