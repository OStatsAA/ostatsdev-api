namespace OStats.Domain.Aggregates.ProjectAggregate.Extensions;

public static class RolesExtensions
{
    public static Role? GetUserRole(this IEnumerable<Role> roles, Guid userId)
    {
        return roles.SingleOrDefault(role => role.UserId == userId);
    }

    public static IReadOnlyList<Guid> GetUsersIdsByAccessLevel(this IEnumerable<Role> roles, AccessLevel accessLevel)
    {
        return roles.Where(role => role.AccessLevel == accessLevel)
                    .Select(role => role.UserId)
                    .ToList();
    }

    public static bool IsUser(this IEnumerable<Role> roles, Guid userId, AccessLevel accessLevel)
    {
        var userRole = roles.GetUserRole(userId);

        if (userRole == null)
        {
            return false;
        }

        return userRole.AccessLevel == accessLevel;
    }

    public static bool IsUserAtLeast(this IEnumerable<Role> roles, Guid userId, AccessLevel accessLevel)
    {
        var userRole = roles.GetUserRole(userId);

        if (userRole == null)
        {
            return false;
        }

        return userRole.AccessLevel <= accessLevel;
    }
}