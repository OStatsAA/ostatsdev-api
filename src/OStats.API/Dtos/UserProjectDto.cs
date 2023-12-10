using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.API.Dtos;

public class UserProjectDto : BaseProjectDto
{
    public Role UserRole { get; }

    public UserProjectDto(Project project, Role userRole) : base(project)
    {
        UserRole = userRole;
    }
}
