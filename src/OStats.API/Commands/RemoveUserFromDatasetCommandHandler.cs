using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class RemoveUserFromDatasetCommandHandler : IRequestHandler<RemoveUserFromDatasetCommand, ICommandResult<bool>>
{
    private readonly Context _context;
    private readonly IValidator<RemoveUserFromDatasetCommand> _validator;

    public RemoveUserFromDatasetCommandHandler(Context context, IValidator<RemoveUserFromDatasetCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<bool>> Handle(RemoveUserFromDatasetCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return new CommandResult<bool>(validation.Errors);
        }

        var dataset = _context.Datasets.Local.Single(dataset => dataset.Id == request.DatasetId);
        var requestor = _context.Users.Local.Single(user => user.AuthIdentity == request.UserAuthId);
        if (dataset.GetUserAccess(requestor.Id) < DatasetAccessLevel.Administrator)
        {
            var error = new ValidationFailure("User", "Requestor cannot grant access to dataset.");
            return new CommandResult<bool>(error);
        }

        dataset.RemoveUserAccess(request.UserId);

        await _context.SaveChangesAsync(cancellationToken);
        return new CommandResult<bool>(true);
    }
}