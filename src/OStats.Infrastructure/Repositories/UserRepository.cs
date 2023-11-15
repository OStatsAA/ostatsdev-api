using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(Context context) : base(context)
    {
    }

    public async Task<User?> FindUserByAuthIdentityAsync(string authIdentity)
    {
        return await _context.Users.Where(user => user.AuthIdentity == authIdentity)
                                   .SingleOrDefaultAsync();
    }

    public async Task<bool> ExistsByAuthIdentityAsync(string authIdentity)
    {
        return await _context.Users.Where(user => user.AuthIdentity == authIdentity)
                                   .AnyAsync();
    }
}