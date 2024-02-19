using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Queries;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class DatasetLinkedProjectsQueryIntegrationTest : BaseIntegrationTest
{
    public DatasetLinkedProjectsQueryIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Shoud_Get_Dataset_By_Id()
    {
        var owner = await context.Users.FirstAsync();
        var dataset = new Dataset(owner.Id, "Title", "Source");
        await context.AddAsync(dataset);
        var project = await context.Projects.FirstAsync();
        project.LinkDataset(dataset.Id, owner.Id);
        await context.SaveChangesAsync();

        var query = new DatasetLinkedProjectsQuery(owner.AuthIdentity, dataset.Id);
        var result = await sender.Send(query);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.ValidationFailures.Should().BeNullOrEmpty();
            result.Value.Should().HaveCount(1);
            if (result.Value is not null)
            {
                result.Value.First().DatasetId.Should().Be(dataset.Id);
                result.Value.First().Project.Id.Should().Be(project.Id);
            }
        }
    }

    [Fact]
    public async Task Shoud_Fail_If_User_Has_No_Access()
    {
        var owner = await context.Users.FirstAsync();
        var dataset = new Dataset(owner.Id, "Title", "Source");
        await context.AddAsync(dataset);
        var project = await context.Projects.FirstAsync();
        project.LinkDataset(dataset.Id, owner.Id);
        await context.SaveChangesAsync();

        var unauthorizedUser = new User("Name", "email@email.com", "unauthorized_authid");
        await context.AddAsync(unauthorizedUser);

        await context.SaveChangesAsync();

        var query = new DatasetLinkedProjectsQuery(unauthorizedUser.AuthIdentity, dataset.Id);
        var result = await sender.Send(query);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ValidationFailures.Should().NotBeEmpty();
            result.Value.Should().BeNull();
        }
    }
}