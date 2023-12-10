using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class UserQueriesIntegrationTest : BaseIntegrationTest
{
    public UserQueriesIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Get_User_Projects_By_User_Id()
    {
        var owner = await context.Users.FirstAsync();
        var projects = new[]{
            new Project(owner.Id, "Project1", "Test"),
            new Project(owner.Id, "Project2", "Test"),
            new Project(owner.Id, "Project3", "Test"),
        };
        await context.AddRangeAsync(projects);

        var user = new User("User", "user@test.com", "auth_id_user");
        await context.AddAsync(user);

        var userProjectsIds = new List<Guid>(2);
        foreach (var userProject in projects.Take(2).AsEnumerable())
        {
            userProject.AddOrUpdateUserRole(user.Id, AccessLevel.ReadOnly);
            userProjectsIds.Add(userProject.Id);
        }

        await context.SaveChangesAsync();

        var result = await userQueries.GetProjectsByUserIdAsync(user.Id);

        using (new AssertionScope())
        {
            result.Should().AllBeOfType<UserProjectDto>();
            result.Should().HaveCount(2);
            result.Select(userProjectDto => userProjectDto.Id)
                  .Should()
                  .BeEquivalentTo(userProjectsIds);

        }
    }
}