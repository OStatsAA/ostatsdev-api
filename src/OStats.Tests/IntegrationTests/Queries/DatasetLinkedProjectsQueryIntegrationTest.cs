using Microsoft.EntityFrameworkCore;
using OStats.API.Queries;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Infrastructure;

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
        var ownerRole = await context.Roles.FindProjectOwnerAsync(project.Id, default);
        project.LinkDataset(dataset.Id, ownerRole);
        await context.SaveChangesAsync();

        var queriedDatasetProjectLinks = await DatasetQueries.GetDatasetLinkedProjectsAsync(context, dataset.Id, default);
        queriedDatasetProjectLinks.Should().HaveCount(1);
        queriedDatasetProjectLinks.First().Dataset.Id.Should().Be(dataset.Id);
    }
}