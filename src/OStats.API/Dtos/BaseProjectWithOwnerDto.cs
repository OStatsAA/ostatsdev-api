using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public class BaseProjectWithOwnersDto : BaseProjectDto
{
    public List<BaseUserDto> Owners { get; }

    public BaseProjectWithOwnersDto(Project project, List<User> owners) : base(project)
    {
        Owners = owners.Select(owner => new BaseUserDto(owner)).ToList();
    }
}