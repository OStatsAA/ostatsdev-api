using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OStats.API.Commands;

namespace OStats.Tests.IntegrationTests.Commands;

public class CleanupDeletedProjectResourcesIntegrationTest : BaseIntegrationTest
{
    public CleanupDeletedProjectResourcesIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Fail_If_Project_Is_Not_Deleted()
    {
        var project = await context.Projects.FirstAsync();
        var command = new CleanupDeletedProjectResourcesCommand { ProjectId = project.Id };
        var result = await serviceProvider.GetRequiredService<CleanupDeletedProjectResourcesCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async Task Should_Fail_If_Project_Is_Not_Found()
    {
        var command = new CleanupDeletedProjectResourcesCommand { ProjectId = Guid.NewGuid() };
        var result = await serviceProvider.GetRequiredService<CleanupDeletedProjectResourcesCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeEmpty();
        }
    }
}