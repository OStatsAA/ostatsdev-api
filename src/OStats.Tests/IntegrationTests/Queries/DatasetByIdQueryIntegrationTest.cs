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
        var owner = await context.Users.FirstAsync();
        var dataset = new Dataset(owner.Id, "Title", "Source");
        await context.AddAsync(dataset);
        var editor = new User("Editor", "editor@email.com", "editor_authid");
        await context.AddAsync(editor);
        dataset.GrantUserAccess(editor.Id, DatasetAccessLevel.Editor, owner.Id);
        await context.SaveChangesAsync();

        var queriedDataset = await DatasetQueries.GetDatasetByIdAsync(context, owner.AuthIdentity, dataset.Id, default);
        using (new AssertionScope())
        {
            queriedDataset.Should().NotBeNull();
            queriedDataset!.Id.Should().Be(dataset.Id);
        }
    }

    [Fact]
    public async Task Shoud_Fail_If_User_Has_No_Access()
    {
        var owner = await context.Users.FirstAsync();
        var dataset = new Dataset(owner.Id, "Title", "Source");
        await context.AddAsync(dataset);
        var editor = new User("Editor", "editor@email.com", "editor_authid2");
        await context.AddAsync(editor);
        dataset.GrantUserAccess(editor.Id, DatasetAccessLevel.Editor, owner.Id);

        var unauthorizedUser = new User("Name", "email@email.com", "unauthorized_authid");
        await context.AddAsync(unauthorizedUser);

        await context.SaveChangesAsync();

        var queriedDataset = await DatasetQueries.GetDatasetByIdAsync(context, unauthorizedUser.AuthIdentity, dataset.Id, default);
        queriedDataset.Should().BeNull();
    }
}