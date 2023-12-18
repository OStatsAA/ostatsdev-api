namespace OStats.Domain.Aggregates.DatasetAggregate;

public enum DatasetAccessLevel
{
    Owner = 40,
    Administrator = 30,
    Editor = 20,
    ReadOnly = 10,
    NoAccess = 0
}