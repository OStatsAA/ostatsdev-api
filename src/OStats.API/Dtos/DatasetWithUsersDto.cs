using System.Text.Json.Serialization;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public record DatasetWithUsersDto : BaseDatasetDto
{
    public List<DatasetUserAccessLevelsDto> DatasetUserAccessLevels { get; init; }

    [JsonConstructor]
    public DatasetWithUsersDto(Dataset dataset, List<User> users) : base(dataset)
    {
        DatasetUserAccessLevels = GetUserAccessLevelDto(dataset, users);
    }

    private List<DatasetUserAccessLevelsDto> GetUserAccessLevelDto(Dataset dataset, List<User> users)
    {
        return dataset.DatasetUserAccessLevels
            .Select(userAccess => new DatasetUserAccessLevelsDto(
                users.Single(u => u.Id == userAccess.UserId), userAccess))
            .ToList();
    }
}