using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.API.Dtos;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class UserDatasetsWithAccessQueryHandler : IRequestHandler<UserDatasetsWithAccessQuery, ICommandResult<List<UserDatasetDto>>>
{
    private readonly Context _context;

    public UserDatasetsWithAccessQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<List<UserDatasetDto>>> Handle(UserDatasetsWithAccessQuery request, CancellationToken cancellationToken)
    {
        var userDatasets = await _context.DatasetsUsersAccessLevels
            .Join(
                _context.Datasets,
                userAccess => userAccess.DatasetId,
                dataset => dataset.Id,
                (userAccess, dataset) => new { dataset, userAccess })
            .Join(
                _context.Users,
                datasetAndUserId => datasetAndUserId.userAccess.UserId,
                user => user.Id,
                (datasetAndUserId, user) => new { datasetAndUserId, user })
            .Where(joined => joined.datasetAndUserId.userAccess.UserId == request.UserId &&
                             joined.user.AuthIdentity == request.UserAuthId)
            .Select(join => new UserDatasetDto(join.datasetAndUserId.dataset, join.datasetAndUserId.userAccess))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new CommandResult<List<UserDatasetDto>>(userDatasets);
    }
}