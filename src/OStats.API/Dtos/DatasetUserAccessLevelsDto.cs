using System.Text.Json.Serialization;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.API.Dtos;

public record DatasetUserAccessLevelsDto : BaseEntityDto
{
    public BaseUserDto User { get; }
    public DatasetAccessLevel DatasetAccessLevel { get; }

    [JsonConstructor]
    public DatasetUserAccessLevelsDto(User user, DatasetUserAccessLevel datasetUserAccessLevel) : base(datasetUserAccessLevel)
    {
        DatasetAccessLevel = datasetUserAccessLevel.AccessLevel;
        User = new BaseUserDto(user);
    }

}