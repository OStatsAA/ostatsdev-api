using OStats.API.Extensions;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Infrastructure;

namespace OStats.API.Common;

public sealed class UserContext
{
    private readonly HttpContext _httpContext;
    private readonly Context _context;
    private User? _user;

    public UserContext(IHttpContextAccessor httpContextAccessor, Context context)
    {
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        if (_user is not null)
        {
            return await Task.FromResult(_user);
        }

        _user = await _context.Users.FindByAuthIdentityAsync(_httpContext.GetUserAuthId(), cancellationToken);
        return _user!;
    }

    public async Task<Guid> GetCurrentUserIdAsync(CancellationToken cancellationToken)
    {
        return (await GetCurrentUserAsync(cancellationToken)).Id;
    }
}