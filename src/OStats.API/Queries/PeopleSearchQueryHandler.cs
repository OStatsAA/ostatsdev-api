using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class PeopleSearchQueryHandler : IRequestHandler<PeopleSearchQuery, ICommandResult<List<BaseUserDto>>>
{
    private readonly Context _context;

    public PeopleSearchQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<ICommandResult<List<BaseUserDto>>> Handle(PeopleSearchQuery request, CancellationToken cancellationToken)
    {
        var people = await _context.Users
            .IgnoreAutoIncludes()
            .AsNoTracking()
            .Where(user => EF.Functions.ILike(user.Name, $"%{request.SearchInput}%"))
            .Select(user => new BaseUserDto(user))
            .ToListAsync(cancellationToken);

        return new CommandResult<List<BaseUserDto>>(people);
    }
}