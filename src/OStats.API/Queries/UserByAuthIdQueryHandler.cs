using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class UserByAuthIdQueryHandler : IRequestHandler<UserByAuthIdQuery, ICommandResult<BaseUserDto>>
{
    private readonly Context _context;

    public UserByAuthIdQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<BaseUserDto>> Handle(UserByAuthIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(user => user.AuthIdentity == request.UserAuthId)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            var error = new ValidationFailure("UserId", "User not found.");
            return new CommandResult<BaseUserDto>(error);
        }

        return new CommandResult<BaseUserDto>(new BaseUserDto(user));
    }
}