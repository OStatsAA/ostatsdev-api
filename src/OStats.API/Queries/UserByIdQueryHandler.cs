using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class UserByIdQueryHandler : IRequestHandler<UserByIdQuery, ICommandResult<BaseUserDto>>
{
    private readonly Context _context;

    public UserByIdQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<BaseUserDto>> Handle(UserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(user => user.Id == request.UserId && user.AuthIdentity == request.UserAuthId)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            var error = new ValidationFailure("UserId", "User not found.");
            return new CommandResult<BaseUserDto>(error);
        }

        return new CommandResult<BaseUserDto>(new BaseUserDto(user));
    }
}