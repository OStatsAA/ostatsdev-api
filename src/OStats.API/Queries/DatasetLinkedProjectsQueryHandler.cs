using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class DatasetLinkedProjectsQueryHandler : IRequestHandler<DatasetLinkedProjectsQuery, ICommandResult<List<DatasetProjectLinkDto>>>
{
    private readonly Context _context;

    public DatasetLinkedProjectsQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<List<DatasetProjectLinkDto>>> Handle(DatasetLinkedProjectsQuery request, CancellationToken cancellationToken)
    {
        var hasAccess = await _context.DatasetsUsersAccessLevels
            .Join(
                _context.Users.IgnoreAutoIncludes(),
                access => access.UserId,
                user => user.Id,
                (access, user) => new { access, user })
            .AsNoTracking()
            .AnyAsync(join => join.user.AuthIdentity == request.UserAuthId &&
                              join.access.DatasetId == request.DatasetId);

        if (!hasAccess)
        {
            var error = new ValidationFailure("UserId", "Unauthorized");
            return new CommandResult<List<DatasetProjectLinkDto>>(error);
        }

        var datasetLinkedProjects = await _context.DatasetsProjectsLinks
            .Join(
                _context.Projects.IgnoreAutoIncludes(),
                link => link.ProjectId,
                project => project.Id,
                (link, project) => new { link, project })
            .Where(join => join.link.DatasetId == request.DatasetId)
            .Select(join => new DatasetProjectLinkDto(join.link, join.project))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new CommandResult<List<DatasetProjectLinkDto>>(datasetLinkedProjects);
    }
}