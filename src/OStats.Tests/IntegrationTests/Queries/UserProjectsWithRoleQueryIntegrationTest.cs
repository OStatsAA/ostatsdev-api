using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.API.Queries;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class UserProjectsWithRoleQueryIntegrationTest : BaseIntegrationTest
{
    public UserProjectsWithRoleQueryIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
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
            userProject.AddOrUpdateUserRole(user.Id, AccessLevel.ReadOnly, owner.Id);
            userProjectsIds.Add(userProject.Id);
        }

        await context.SaveChangesAsync();

        var queriedUserProjects = await UserQueries.GetUserProjectsAsync(context, user.AuthIdentity, user.Id);

        queriedUserProjects.Should().AllBeOfType<UserProjectDto>();
        queriedUserProjects.Should().HaveCount(2);
        queriedUserProjects.Select(userProjectDto => userProjectDto.Id).Should().BeEquivalentTo(userProjectsIds);
    }
}