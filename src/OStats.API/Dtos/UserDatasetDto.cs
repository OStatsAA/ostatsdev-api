using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.API.Dtos;

public class UserDatasetDto : BaseDatasetDto
{
    public DatasetUserAccessLevel UserAccess { get; }

    public UserDatasetDto(Dataset dataset, DatasetUserAccessLevel userAccess) : base(dataset)
    {
        UserAccess = userAccess;
    }
}
