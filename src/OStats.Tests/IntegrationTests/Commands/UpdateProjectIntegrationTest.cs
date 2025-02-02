using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OStats.API.Commands;
using OStats.API.Dtos;
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
        var project = Project.Create("Test Should_Update_Project", "Description", user.Id, out var userRole);
        await context.AddAsync(project);
        await context.AddAsync(userRole);
        await context.SaveChangesAsync();
        var previousCreatedAtDatetime = project.CreatedAt;
        var previousLastUpdatedAtDatetime = project.LastUpdatedAt;
        var editedTitle = "Edited Title";
        var editedDescription = "Edited description";
        var command = new UpdateProjectCommand(project.Id, user.Id, editedTitle, project.LastUpdatedAt, editedDescription);
        var (result, baseProjectDto) = await serviceProvider.GetRequiredService<UpdateProjectCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();

            baseProjectDto.Should().NotBeNull().And.BeOfType<BaseProjectDto>();
            baseProjectDto!.CreatedAt.Should().Be(previousCreatedAtDatetime);
            baseProjectDto.LastUpdatedAt.Should().BeAfter(previousLastUpdatedAtDatetime);
            baseProjectDto.Title.Should().Be(editedTitle);
            baseProjectDto.Description.Should().Be(editedDescription);
        }
    }

    [Fact]
    public async Task Should_Fail_If_LastUpdatedAt_Does_Not_Match_Project()
    {
        var user = await context.Users.FirstAsync();
        var project = Project.Create("Test Should_Fail_If_LastUpdatedAt_Does_Not_Match_Project", "Description", user.Id, out var userRole);
        await context.AddAsync(project);
        await context.AddAsync(userRole);
        await context.SaveChangesAsync();
        var previousLastUpdatedAtDatetime = project.LastUpdatedAt;
        var editedTitle = "Edited Title";
        var editedDescription = "Edited description";
        project.SetTitle("Bypassed edition", userRole);
        await context.SaveChangesAsync();
        var command = new UpdateProjectCommand(project.Id, user.Id, editedTitle, previousLastUpdatedAtDatetime, editedDescription);
        var (result, baseProjectDto) = await serviceProvider.GetRequiredService<UpdateProjectCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
            baseProjectDto.Should().BeNull();
        }
    }

}