using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Tests.IntegrationTests.Commands;

public class UpdateProjectIntegrationTest : BaseIntegrationTest
{
    public UpdateProjectIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Update_Project()
    {
        var user = await context.Users.FirstAsync();
        var project = new Project(user.Id, "Test Should_Update_Project", "Description");
        await context.AddAsync(project);
        await context.SaveEntitiesAsync();
        var previousCreatedAtDatetime = project.CreatedAt;
        var previousLastUpdatedAtDatetime = project.LastUpdatedAt;
        var editedTitle = "Edited Title";
        var editedDescription = "Edited description";
        var command = new UpdateProjectCommand(project.Id, user.AuthIdentity, editedTitle, project.LastUpdatedAt, editedDescription);
        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.ValidationFailures.Should().BeNull();
            result.Value.Should().NotBeNull().And.BeOfType<Project>();
            if (result.Value is not null)
            {
                result.Value.CreatedAt.Should().Be(previousCreatedAtDatetime);
                result.Value.LastUpdatedAt.Should().BeAfter(previousLastUpdatedAtDatetime);
                result.Value.Title.Should().Be(editedTitle);
                result.Value.Description.Should().Be(editedDescription);
            }
        }
    }

    [Fact]
    public async Task Should_Fail_If_LastUpdatedAt_Does_Not_Match_Project()
    {
        var user = await context.Users.FirstAsync();
        var project = new Project(user.Id, "Test Should_Fail_If_LastUpdatedAt_Does_Not_Match_Project", "Description");
        await context.AddAsync(project);
        await context.SaveEntitiesAsync();
        var previousLastUpdatedAtDatetime = project.LastUpdatedAt;
        var editedTitle = "Edited Title";
        var editedDescription = "Edited description";
        project.Title = "Bypassed edition";
        await context.SaveEntitiesAsync();
        var command = new UpdateProjectCommand(project.Id, user.AuthIdentity, editedTitle, previousLastUpdatedAtDatetime, editedDescription);
        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.Value.Should().BeNull();
            result.ValidationFailures.Should().NotBeNull();
            if (result.ValidationFailures is not null)
            {
                result.ValidationFailures.Where(vf => vf.PropertyName == "LastUpdatedAt").Should().HaveCount(1);
            }
        }
    }

}