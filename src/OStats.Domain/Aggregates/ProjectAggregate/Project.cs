using OStats.Domain.Common;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;

namespace OStats.Domain.Aggregates.ProjectAggregate;

public class Project : Entity, IAggregateRoot
{
    public string Title { get; }
    public string? Description { get; }
    private readonly List<Role> _roles = new List<Role>();
    public IReadOnlyCollection<Role> Roles => _roles;
    private readonly List<DatasetConfiguration> _datasetsConfigs = new List<DatasetConfiguration>();
    public IReadOnlyCollection<DatasetConfiguration> DatasetsConfigs => _datasetsConfigs;

    private Project(string title, string? description)
    {
        Title = title;
        Description = description;
    }
    
    public Project(Guid ownerId, string title, string? description)
    {
        Title = title;
        Description = description;
        AddOrUpdateUserRole(ownerId, AccessLevel.Owner);
    }

    public void AddOrUpdateUserRole(Guid userId, AccessLevel accessLevel)
    {
        var userRole = _roles.GetUserRole(userId);

        if (userRole != null)
        {
            userRole.AccessLevel = accessLevel;
            return;
        }

        _roles.Add(new Role(Id, userId, accessLevel));
    }

    public void RemoveUserRole(Guid userId)
    {
        var userRole = _roles.GetUserRole(userId);
        if (userRole != null)
        {
            _roles.Remove(userRole);
        }
    }

    public Role? GetUserRole(Guid userId)
    {
        return _roles.Find(role => role.UserId == userId);
    }

    public void AddDatasetConfiguration(DatasetConfiguration datasetConfiguration)
    {
        if (_datasetsConfigs.Any(config => config.Title == datasetConfiguration.Title))
        {
            throw new ArgumentException("Cannot add another dataset with same name to project.");
        }

        _datasetsConfigs.Add(datasetConfiguration);
    }

    public void UpdateDatasetConfiguration(DatasetConfiguration datasetConfiguration)
    {
        var currentDatasetConfig = _datasetsConfigs.SingleOrDefault(config => config.Id == datasetConfiguration.Id);
        currentDatasetConfig = datasetConfiguration;
    }

    public void RemoveDatasetConfiguration(Guid datasetConfigId)
    {
        var datasetConfig = _datasetsConfigs.SingleOrDefault(config => config.Id == datasetConfigId);
        if (datasetConfig != null)
        {
            _datasetsConfigs.Remove(datasetConfig);
        }
    }
}