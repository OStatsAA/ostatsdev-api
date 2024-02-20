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

        var queriedDatasetProjectLinks = await DatasetQueries.GetDatasetLinkedProjectsAsync(context, dataset.Id);
        queriedDatasetProjectLinks.Should().HaveCount(1);
        queriedDatasetProjectLinks.First().DatasetId.Should().Be(dataset.Id);
    }
}