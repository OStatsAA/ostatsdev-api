using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OStats.API.Commands;
using OStats.API.Dtos;

namespace OStats.Tests.IntegrationTests.Commands;

public class CreateProjectIntegrationTest : BaseIntegrationTest
{
    public CreateProjectIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Add_Project_To_Database()
    {
        var beforeCommandTime = DateTime.UtcNow;
        var existingUser = await context.Users.FirstAsync();
        var command = new CreateProjectCommand(existingUser.AuthIdentity, "Test", "Test");
        var (result, baseProjectDto) = await serviceProvider.GetRequiredService<CreateProjectCommandHandler>().Handle(command, default);
        var afterCommandTime = DateTime.UtcNow;

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();

            baseProjectDto.Should().NotBeNull().And.BeOfType<BaseProjectDto>();
            baseProjectDto!.CreatedAt.Should().BeAfter(beforeCommandTime).And.BeBefore(afterCommandTime);
            baseProjectDto.LastUpdatedAt.Should().BeAfter(beforeCommandTime).And.BeBefore(afterCommandTime);
        }
    }

    [Fact]
    public async Task Should_Fail_If_User_Does_Not_Exists()
    {
        var command = new CreateProjectCommand("Test", "test@test.com", "An authid that clearly doesnt exist");
        var (result, baseProjectDto) = await serviceProvider.GetRequiredService<CreateProjectCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeEmpty();

            baseProjectDto.Should().BeNull();
        }
    }
}