using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Commands;

public class DeleteProjectIntegrationTest : BaseIntegrationTest
{
    public DeleteProjectIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Fail_If_User_Is_Not_Owner()
    {
        var project = await context.Projects.FirstAsync();
        var ownerId = project.Roles.GetUsersIdsByAccessLevel(AccessLevel.Owner).First();
        var user = new User("Test User", "test@test.com", "test_user_authid");
        await context.AddAsync(user);
        project.AddOrUpdateUserRole(user.Id, AccessLevel.Editor, ownerId);
        await context.SaveChangesAsync();
        var command = new DeleteProjectCommand(user.AuthIdentity, project.Id);
        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ValidationFailures.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async Task Should_Remove_Project_From_Database()
    {
        var existingUser = await context.Users.FirstAsync();
        var project = new Project(existingUser.Id, "To Be Deleted", "No Description");
        context.Projects.Add(project);
        context.SaveChanges();

        var command = new DeleteProjectCommand(existingUser.AuthIdentity, project.Id);
        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.ValidationFailures.Should().BeNull();
            context.Users.Any(u => u.Id == existingUser.Id).Should().BeTrue();
            context.Projects.Any(p => p.Id == project.Id).Should().BeFalse();
            context.Roles.Any(r => r.ProjectId == project.Id).Should().BeFalse();
        }
    }
}