using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.API.Queries;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class UserDatasetsWithAccessQueryIntegrationTest : BaseIntegrationTest
{
    public UserDatasetsWithAccessQueryIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Get_User_Datasets_By_User_Id()
    {
        var owner = await context.Users.FirstAsync();
        var datasets = new[]{
            new Dataset(owner.Id, "Dataset1", "Test"),
            new Dataset(owner.Id, "Dataset2", "Test"),
            new Dataset(owner.Id, "Dataset3", "Test"),
        };
        await context.AddRangeAsync(datasets);

        var user = new User("User", "user@test.com", "auth_id_user");
        await context.AddAsync(user);

        var userDatasetsIds = new List<Guid>(2);
        foreach (var userDataset in datasets.Take(2).AsEnumerable())
        {
            userDataset.GrantUserAccess(user.Id, DatasetAccessLevel.ReadOnly);
            userDatasetsIds.Add(userDataset.Id);
        }

        await context.SaveChangesAsync();

        var query = new UserDatasetsWithAccessQuery(user.AuthIdentity, user.Id);
        var result = await sender.Send(query);

        using (new AssertionScope())
        {
            result.Value.Should().NotBeNullOrEmpty();
            if (result.Value is not null)
            {
                result.Value.Should().AllBeOfType<UserDatasetDto>();
                result.Value.Should().HaveCount(2);
                result.Value.Select(userDatasetDto => userDatasetDto.Id)
                      .Should()
                      .BeEquivalentTo(userDatasetsIds);
            }
        }
    }
}