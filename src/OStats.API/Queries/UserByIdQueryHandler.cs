using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class UserByIdQueryHandler : IRequestHandler<UserByIdQuery, ICommandResult<User>>
{
    private readonly Context _context;

    public UserByIdQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<User>> Handle(UserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(user => user.Id == request.UserId && user.AuthIdentity == request.UserAuthId)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            var error = new ValidationFailure("UserId", "User not found.");
            return new CommandResult<User>(error);
        }

        return new CommandResult<User>(user);
    }
}