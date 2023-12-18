using FluentValidation;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class CreateDatasetCommandHandler : IRequestHandler<CreateDatasetCommand, ICommandResult<Dataset>>
{
    private readonly Context _context;
    private readonly IValidator<CreateDatasetCommand> _validator;

    public CreateDatasetCommandHandler(Context context, IValidator<CreateDatasetCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<Dataset>> Handle(CreateDatasetCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return new CommandResult<Dataset>(validation.Errors);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var dataset = new Dataset(user.Id, command.Title, command.Source, command.Description);

        await _context.AddAsync(dataset);
        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<Dataset>(dataset);
    }

}