using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.DatasetAggregate;

public class Dataset : Entity, IAggregateRoot
{
    public string Title { get; set; }
    public string Source { get; set; }
    public string? Description { get; set; }
    private readonly List<DatasetUserAccessLevel> _datasetUsersAccessesLevels = new List<DatasetUserAccessLevel>();
    public IReadOnlyCollection<DatasetUserAccessLevel> DatasetUserAccessLevels => _datasetUsersAccessesLevels;

    private Dataset(string title, string source)
    {
        Title = title;
        Source = source;
    }

    public Dataset(Guid ownerId, string title, string source, string? description = null)
    {
        Title = title;
        Source = source;
        Description = description;
        GrantUserAccess(ownerId, DatasetAccessLevel.Owner);
    }

    public void GrantUserAccess(Guid userId, DatasetAccessLevel accessLevel)
    {
        var datasetUserAccess = new DatasetUserAccessLevel(Id, userId, accessLevel);
        _datasetUsersAccessesLevels.Add(datasetUserAccess);
    }

    public void RemoveUserAccess(Guid userId)
    {
        var userAccess = _datasetUsersAccessesLevels
            .Where(userAccess => userAccess.UserId == userId)
            .Single();
        _datasetUsersAccessesLevels.Remove(userAccess);
    }

    public DatasetAccessLevel GetUserAccess(Guid userId)
    {
        return _datasetUsersAccessesLevels
            .Where(userAccess => userAccess.UserId == userId)
            .Select(userAccess => userAccess.AccessLevel)
            .SingleOrDefault(DatasetAccessLevel.NoAccess);
    }
}