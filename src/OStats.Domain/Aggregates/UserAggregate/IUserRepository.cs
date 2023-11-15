using OStats.Domain.Common;

namespace OStats.Domain.Aggregates.UserAggregate;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindUserByAuthIdentityAsync(string authIdentity);
    Task<bool> ExistsByAuthIdentityAsync(string authIdentity);
}