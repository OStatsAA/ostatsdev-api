using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Queries;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class DatasetByIdQueryIntegrationTest : BaseIntegrationTest
{
    public DatasetByIdQueryIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Shoud_Get_Dataset_By_Id()
    {
        var user = await context.Users.FirstAsync();
        var dataset = new Dataset(user.Id, "Title", "Source");
        await context.AddAsync(dataset);
        await context.SaveChangesAsync();

        var query = new DatasetByIdQuery(user.AuthIdentity, dataset.Id);
        var result = await sender.Send(query);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.ValidationFailures.Should().BeNullOrEmpty();
            result.Value.Should().BeOfType<Dataset>();
            if (result.Value is not null)
            {
                result.Value.Should().BeEquivalentTo(dataset);
                result.Value.DatasetUserAccessLevels.Should().HaveCount(1);
            }
        }
    }

    [Fact]
    public async Task Shoud_Fail_If_User_Has_No_Access()
    {
        var user = await context.Users.FirstAsync();
        var dataset = new Dataset(user.Id, "Title", "Source");
        await context.AddAsync(dataset);

        var unauthorizedUser = new User("Name", "email@email.com", "unauthorized_authid");
        await context.AddAsync(unauthorizedUser);

        await context.SaveChangesAsync();

        var query = new DatasetByIdQuery(unauthorizedUser.AuthIdentity, dataset.Id);
        var result = await sender.Send(query);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ValidationFailures.Should().NotBeEmpty();
            result.Value.Should().BeNull();
        }
    }
}