using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public record BaseProjectWithOwnersDto : BaseProjectDto
{
    public required IReadOnlyCollection<BaseUserDto> Owners { get; init; }

    public BaseProjectWithOwnersDto(Project project, List<User> owners) : base(project)
    {
        Owners = owners.Select(owner => new BaseUserDto(owner)).ToList();
    }
}