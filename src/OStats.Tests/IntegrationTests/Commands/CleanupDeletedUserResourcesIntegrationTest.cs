using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OStats.API.Commands;

namespace OStats.Tests.IntegrationTests.Commands;

public class CleanupDeletedUserResourcesIntegrationTest : BaseIntegrationTest
{
    public CleanupDeletedUserResourcesIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Fail_If_User_Is_Not_Deleted()
    {
        var user = await context.Users.FirstAsync();
        var command = new CleanupDeletedUserResourcesCommand { UserId = user.Id };
        var result = await serviceProvider.GetRequiredService<CleanupDeletedUserResourcesCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async Task Should_Fail_If_User_Is_Not_Found()
    {
        var command = new CleanupDeletedUserResourcesCommand { UserId = Guid.NewGuid() };
        var result = await serviceProvider.GetRequiredService<CleanupDeletedUserResourcesCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeEmpty();
        }
    }
}