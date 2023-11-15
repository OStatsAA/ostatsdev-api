using Microsoft.EntityFrameworkCore;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class UserQueries : IUserQueries
{
    private readonly Context _context;
    private IQueryable<User> _users => _context.Users.AsNoTracking();

    public UserQueries(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _users.Where(user => user.Id == id)
                           .SingleOrDefaultAsync();
    }

    public async Task<User?> GetUserByNameAsync(string name)
    {
        return await _users.Where(user => user.Name == name)
                           .SingleOrDefaultAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _users.Where(user => user.Email == email)
                           .SingleOrDefaultAsync();
    }

    public async Task<User?> GetUserByAuthIdentity(string authIdentity)
    {
        return await _users.Where(user => user.AuthIdentity == authIdentity)
                           .SingleOrDefaultAsync();
    }
}