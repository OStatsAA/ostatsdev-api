using FluentValidation;
using MediatR;
using OStats.API.Common;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Infrastructure;

namespace OStats.API.Commands;

public class UnlinkProjectToDatasetCommandHandler : IRequestHandler<UnlinkProjectToDatasetCommand, ICommandResult<bool>>
{
    private readonly Context _context;
    private readonly IValidator<UnlinkProjectToDatasetCommand> _validator;

    public UnlinkProjectToDatasetCommandHandler(Context context, IValidator<UnlinkProjectToDatasetCommand> validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ICommandResult<bool>> Handle(UnlinkProjectToDatasetCommand command, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return new CommandResult<bool>(validation.Errors);
        }

        var user = _context.Users.Local.Where(user => user.AuthIdentity == command.UserAuthId).Single();
        var project = _context.Projects.Local.Where(project => project.Id == command.ProjectId).Single();
        var dataset = _context.Datasets.Local.Where(dataset => dataset.Id == command.DatasetId).Single();

        if(project.Roles.IsUserAtLeast(user.Id, AccessLevel.Editor))
        {
            project.UnlinkDataset(dataset.Id);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new CommandResult<bool>(true);
    }

}