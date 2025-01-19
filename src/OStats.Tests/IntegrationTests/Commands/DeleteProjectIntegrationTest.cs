using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        var command = new DeleteProjectCommand(user.Id, project.Id);
        var result = await serviceProvider.GetRequiredService<DeleteProjectCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeEmpty();
            var isDeleted = !await context.Projects.AnyAsync(p => p.Id == project.Id);
            isDeleted.Should().BeFalse();
        }
    }

    [Fact]
    public async Task Should_Remove_Project_From_Database()
    {
        var existingUser = await context.Users.FirstAsync();
        var project = new Project(existingUser.Id, "To Be Deleted", "No Description");
        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();

        var command = new DeleteProjectCommand(existingUser.Id, project.Id);
        var result = await serviceProvider.GetRequiredService<DeleteProjectCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();
            context.Users.Any(u => u.Id == existingUser.Id).Should().BeTrue();
            context.Projects.Any(p => p.Id == project.Id).Should().BeFalse();
        }
    }
}