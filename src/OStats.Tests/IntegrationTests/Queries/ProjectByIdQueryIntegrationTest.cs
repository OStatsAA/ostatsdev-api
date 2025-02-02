using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Queries;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class ProjectByIdQueryIntegrationTest : BaseIntegrationTest
{
    public ProjectByIdQueryIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Get_Project_By_Id()
    {
        var user = await context.Users.FirstAsync();
        var project = Project.Create("Test", "Test description", user.Id, out var userRole);
        await context.AddAsync(project);
        await context.AddAsync(userRole);
        await context.SaveChangesAsync();

        var queriedProject = await ProjectQueries.GetProjectByIdAsync(context, user.Id, project.Id, default);

        using(new AssertionScope())
        {
            queriedProject.Should().NotBeNull();
            queriedProject!.Id.Should().Be(project.Id);
        }
    }

    [Fact]
    public async Task Shoud_Fail_If_User_Has_No_Access()
    {
        var user = await context.Users.FirstAsync();
        var project = Project.Create("Test", "Test description", user.Id, out var userRole);
        await context.AddAsync(project);
        await context.AddAsync(userRole);
        await context.SaveChangesAsync();

        var unauthorized = new User("Name", "name@email.com", "unauthorized_authid");
        await context.AddAsync(unauthorized);
        await context.SaveChangesAsync();

        var queriedProject = await ProjectQueries.GetProjectByIdAsync(context, unauthorized.Id, project.Id, default);

        queriedProject.Should().BeNull();
    }
}