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
        var project = new Project(user.Id, "Test", "Test description");
        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();

        var query = new ProjectByIdQuery(user.AuthIdentity, project.Id);
        var queryResult = await sender.Send(query);

        using (new AssertionScope())
        {
            queryResult.Should().NotBeNull();
            queryResult.Success.Should().BeTrue();
            queryResult.ValidationFailures.Should().BeNullOrEmpty();
        }
    }

    [Fact]
    public async Task Shoud_Fail_If_User_Has_No_Access()
    {
        var user = await context.Users.FirstAsync();
        var project = new Project(user.Id, "Test", "Test description");
        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();

        var unauthorized = new User("Name", "name@email.com", "unauthorized_authid");
        await context.AddAsync(unauthorized);
        await context.SaveChangesAsync();

        var query = new ProjectByIdQuery(unauthorized.AuthIdentity, project.Id);
        var queryResult = await sender.Send(query);

        using (new AssertionScope())
        {
            queryResult.Success.Should().BeFalse();
            queryResult.ValidationFailures.Should().NotBeEmpty();
            queryResult.Value.Should().BeNull();
        }
    }
}