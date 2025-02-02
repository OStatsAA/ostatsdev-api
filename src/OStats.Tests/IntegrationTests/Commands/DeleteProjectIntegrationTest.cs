using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Infrastructure;

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
        var ownerRole = await context.Roles.FindProjectOwnerAsync(project.Id, CancellationToken.None);
        var user = new User("Test User", "test@test.com", "test_user_authid");
        await context.AddAsync(user);
        var (_, userRole) = project.CreateUserRole(user.Id, AccessLevel.Editor, ownerRole);
        await context.AddAsync(userRole!);
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
        var project = Project.Create("To Be Deleted", "No Description", existingUser.Id, out var existingUserRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(existingUserRole);
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