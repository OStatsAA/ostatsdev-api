namespace OStats.Domain.Common;

public abstract class Entity
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

    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}