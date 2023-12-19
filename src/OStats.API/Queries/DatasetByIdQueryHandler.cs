using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API.Queries;

public class DatasetByIdQueryHandler : IRequestHandler<DatasetByIdQuery, ICommandResult<Dataset>>
{
    private readonly Context _context;

    public DatasetByIdQueryHandler(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICommandResult<Dataset>> Handle(DatasetByIdQuery request, CancellationToken cancellationToken)
    {
        var dataset = await _context.DatasetsUsersAccessLevels
            .Join(
                _context.Datasets,
                userAccess => userAccess.DatasetId,
                dataset => dataset.Id,
                (userAccess, dataset) => new { dataset, userAccess.UserId })
            .Join(
                _context.Users,
                datasetAndUserId => datasetAndUserId.UserId,
                user => user.Id,
                (datasetAndUserId, user) => new { datasetAndUserId, user })
            .Where(joined => joined.datasetAndUserId.dataset.Id == request.DatasetId &&
                             joined.user.AuthIdentity == request.UserAuthId)
            .Select(joined => joined.datasetAndUserId.dataset)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (dataset is null)
        {
            var error = new ValidationFailure("DatasetId", "Dataset not found.");
            return new CommandResult<Dataset>(error);
        }

        return new CommandResult<Dataset>(dataset);
    }
}