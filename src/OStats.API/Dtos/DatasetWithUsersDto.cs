using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public record DatasetWithUsersDto : BaseDatasetDto
{
    public List<DatasetUserAccessLevelsDto> DatasetUserAccessLevels { get; }
    public DatasetWithUsersDto(Dataset dataset, List<User> users) : base(dataset)
    {
        DatasetUserAccessLevels = dataset.DatasetUserAccessLevels
            .Select(userAccess => new DatasetUserAccessLevelsDto(users.Single(u => u.Id == userAccess.UserId), userAccess))
            .ToList();
    }
}