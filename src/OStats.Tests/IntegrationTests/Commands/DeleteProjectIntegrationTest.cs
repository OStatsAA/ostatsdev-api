using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;

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
        var command = new DeleteProjectCommand(project.Id, Guid.NewGuid());
        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Should().BeFalse();
        }
    }

    [Fact]
    public async Task Should_Remove_Project_From_Database()
    {
        var existingUser = await context.Users.FirstAsync();
        var project = new Project(existingUser.Id, "To Be Deleted", "No Description");
        context.Projects.Add(project);
        context.SaveChanges();

        var command = new DeleteProjectCommand(project.Id, existingUser.Id);
        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Should().BeTrue();
            context.Users.Any(u => u.Id == existingUser.Id).Should().BeTrue();
            context.Projects.Any(p => p.Id == project.Id).Should().BeFalse();
            context.Roles.Any(r => r.ProjectId == project.Id).Should().BeFalse();
        }
    }
}