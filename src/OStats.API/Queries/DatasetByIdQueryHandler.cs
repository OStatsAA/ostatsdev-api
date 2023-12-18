using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

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
        var user = await _context.Users.FindByAuthIdentityAsync(request.UserAuthId, cancellationToken);
        if (user is null)
        {
            var error = new ValidationFailure("UserAuthId", "User not found.");
            return new CommandResult<Dataset>(error);
        }

        var dataset = await _context.Datasets
            .Include(dataset => dataset.DatasetUserAccessLevels)
            .Where(dataset => dataset.Id == request.DatasetId && dataset.DatasetUserAccessLevels.Any(userAccess => userAccess.UserId == user.Id))
            .SingleOrDefaultAsync(cancellationToken);

        if (dataset is null)
        {
            var error = new ValidationFailure("DatasetId", "Dataset not found.");
            return new CommandResult<Dataset>(error);
        }

        return new CommandResult<Dataset>(dataset);
    }
}