using MediatR;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;

namespace OStats.API.Commands;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly IProjectRepository _projectRepository;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.FindByIdAsync(request.ProjectId);
        if (project == null)
        {
            return false;
        }

        var isUserOwner = project.Roles.IsUser(request.UserId, AccessLevel.Owner);
        if (!isUserOwner)
        {
            return false;
        }

        _projectRepository.Delete(project);

        return await _projectRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}