using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OStats.Infrastructure")]

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

    public DateTime CreatedAt { get; internal set; }
    public DateTime LastUpdatedAt { get; internal set; }
    public bool IsDeleted { get; internal set; }
}