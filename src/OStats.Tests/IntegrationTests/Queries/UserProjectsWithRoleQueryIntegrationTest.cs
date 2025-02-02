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
            Project.Create("Project1", "Test", owner.Id, out var ownerRole0),
            Project.Create("Project2", "Test", owner.Id, out var ownerRole1),
            Project.Create("Project3", "Test", owner.Id, out var ownerRole2),
        };
        await context.AddRangeAsync(projects);
        await context.AddRangeAsync([ownerRole0, ownerRole1, ownerRole2]);

        var user = new User("User", "user@test.com", "auth_id_user");
        await context.AddAsync(user);

        var userProjectsIds = new List<Guid>(2);
        var (_, userReadOnly0) = projects[0].CreateUserRole(user.Id, AccessLevel.ReadOnly, ownerRole0);
        await context.AddAsync(userReadOnly0!);
        userProjectsIds.Add(projects[0].Id);
        var (_, userReadOnly1) = projects[1].CreateUserRole(user.Id, AccessLevel.ReadOnly, ownerRole1);
        await context.AddAsync(userReadOnly1!);
        userProjectsIds.Add(projects[1].Id);

        await context.SaveChangesAsync();

        var queriedUserProjects = await UserQueries.GetUserProjectsAsync(context, user.Id, user.Id, default);

        queriedUserProjects.Should().AllBeOfType<UserProjectDto>();
        queriedUserProjects.Should().HaveCount(2);
        queriedUserProjects.Select(userProjectDto => userProjectDto.Id).Should().BeEquivalentTo(userProjectsIds);
    }
}