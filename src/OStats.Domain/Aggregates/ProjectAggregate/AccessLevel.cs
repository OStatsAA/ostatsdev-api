namespace OStats.Domain.Aggregates.ProjectAggregate;

public enum AccessLevel
{
    Owner = 100,
    Administrator = 90,
    Editor = 80,
    ReadOnly = 50,
    NoAccess = 0
}