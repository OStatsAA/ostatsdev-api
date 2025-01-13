using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OStats.API.Commands;

namespace OStats.Tests.IntegrationTests.Commands;

public class CleanupDeletedDatasetResourcesIntegrationTest : BaseIntegrationTest
{
    public CleanupDeletedDatasetResourcesIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Fail_If_Dataset_Is_Not_Deleted()
    {
        var dataset = await context.Datasets.FirstAsync();
        var command = new CleanupDeletedDatasetResourcesCommand { DatasetId = dataset.Id };
        var result = await serviceProvider.GetRequiredService<CleanupDeletedDatasetResourcesCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async Task Should_Fail_If_Dataset_Is_Not_Found()
    {
        var command = new CleanupDeletedDatasetResourcesCommand { DatasetId = Guid.NewGuid() };
        var result = await serviceProvider.GetRequiredService<CleanupDeletedDatasetResourcesCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeEmpty();
        }
    }
}