using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Queries;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class GetDatasetUsersIntegrationTest : BaseIntegrationTest
{
    public GetDatasetUsersIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Shoud_Get_Dataset_Users()
    {
        var owner = await context.Users.FirstAsync();
        var dataset = new Dataset(owner.Id, "Title", "Source");
        await context.AddAsync(dataset);
        var editor = new User("Editor", "editor@email.com", "editor_authid");
        await context.AddAsync(editor);
        dataset.GrantUserAccess(editor.Id, DatasetAccessLevel.Editor, owner.Id);
        await context.SaveChangesAsync();

        var datasetUsers = await DatasetQueries.GetDatasetUsersAsync(context, dataset.Id, default);

        using (new AssertionScope())
        {
            datasetUsers.Should().NotBeNullOrEmpty();
            datasetUsers.Should().HaveCount(2);
        }
    }
}