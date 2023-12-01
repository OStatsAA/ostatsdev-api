using FluentAssertions.Execution;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;

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
        var result = await sender.Send(command);
        var afterCommandTime = DateTime.UtcNow;

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.ValidationFailures.Should().BeNull();
            result.Value.Should().NotBeNull().And.BeOfType<Project>();
            result.Value?.CreatedAt.Should().BeAfter(beforeCommandTime).And.BeBefore(afterCommandTime);
            result.Value?.LastUpdatedAt.Should().BeAfter(beforeCommandTime).And.BeBefore(afterCommandTime);
            result.Value?.Roles.IsUser(existingUser.Id, AccessLevel.Owner).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Should_Fail_If_User_Does_Not_Exists()
    {
        var command = new CreateProjectCommand("Test", "test@test.com", "An authid that clearly doesnt exist");
        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.Value.Should().BeNull();
            result.ValidationFailures.Should().AllBeOfType<ValidationFailure>();
        }
    }
}