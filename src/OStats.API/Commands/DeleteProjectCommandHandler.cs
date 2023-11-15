using MediatR;
using OStats.Domain.Aggregates.ProjectAggregate;

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

        var userRole = project.GetUserRole(request.UserId);
        if (userRole == null || userRole.AccessLevel != AccessLevel.Owner)
        {
            return false;
        }

        _projectRepository.Delete(project);

        return await _projectRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}