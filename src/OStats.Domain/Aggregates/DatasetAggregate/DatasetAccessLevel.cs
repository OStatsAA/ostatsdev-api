namespace OStats.Domain.Aggregates.DatasetAggregate;

public enum DatasetAccessLevel
{
    Owner = 100,
    Administrator = 90,
    Editor = 80,
    ReadOnly = 50,
    NoAccess = 0
}