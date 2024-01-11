using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class AddUserToDatasetCommandHandler : IRequestHandler<AddUserToDatasetCommand, ICommandResult<bool>>
{
    private readonly Context _context;
    private readonly IValidator<AddUserToDatasetCommand> _validator;

    public AddUserToDatasetCommandHandler(Context context, IValidator<AddUserToDatasetCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
    public async Task<ICommandResult<bool>> Handle(AddUserToDatasetCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return new CommandResult<bool>(validation.Errors);
        }

        var dataset = _context.Datasets.Local.Single(project => project.Id == request.DatasetId);
        var requestor = _context.Users.Local.Single(user => user.AuthIdentity == request.UserAuthId);
        if (dataset.GetUserAccess(requestor.Id) < DatasetAccessLevel.Administrator)
        {
            var error = new ValidationFailure("User", "Requestor cannot grant access to dataset.");
            return new CommandResult<bool>(error);
        }

        dataset.GrantUserAccess(request.UserId, request.AccessLevel);

        await _context.SaveChangesAsync(cancellationToken);
        return new CommandResult<bool>(true);
    }
}
