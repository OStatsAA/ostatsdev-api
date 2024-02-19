using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.API.Dtos;

namespace OStats.Tests.IntegrationTests.Commands;

public class CreateUserIntegrationTest : BaseIntegrationTest
{
    public CreateUserIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Add_User_To_Database()
    {
        var beforeCommandTime = DateTime.UtcNow;
        var command = new CreateUserCommand("Test", "test@test.com", "auth_id_string");
        var (result, baseUserDto) = await sender.Send(command);
        var afterCommandTime = DateTime.UtcNow;

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();

            baseUserDto.Should().NotBeNull().And.BeOfType<BaseUserDto>();
            baseUserDto.CreatedAt.Should().BeAfter(beforeCommandTime).And.BeBefore(afterCommandTime);
            baseUserDto.LastUpdatedAt.Should().BeAfter(beforeCommandTime).And.BeBefore(afterCommandTime);
        }
    }

    [Fact]
    public async Task Should_Fail_If_AuthId_Already_ExistsAsync()
    {
        var existingUser = await context.Users.FirstAsync();
        var command = new CreateUserCommand("Test", "test@test.com", existingUser.AuthIdentity);
        var (result, baseUserDto) = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
            baseUserDto.Should().BeNull();
        }
    }
}