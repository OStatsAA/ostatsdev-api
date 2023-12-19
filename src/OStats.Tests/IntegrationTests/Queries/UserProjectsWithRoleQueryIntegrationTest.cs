using FluentAssertions.Execution;
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
            userProject.AddOrUpdateUserRole(user.Id, AccessLevel.ReadOnly);
            userProjectsIds.Add(userProject.Id);
        }

        await context.SaveChangesAsync();

        var query = new UserProjectsWithRoleQuery(user.AuthIdentity, user.Id);
        var result = await sender.Send(query);

        using (new AssertionScope())
        {
            result.Value.Should().NotBeNullOrEmpty();
            if (result.Value is not null)
            {
                result.Value.Should().AllBeOfType<UserProjectDto>();
                result.Value.Should().HaveCount(2);
                result.Value.Select(userProjectDto => userProjectDto.Id)
                      .Should()
                      .BeEquivalentTo(userProjectsIds);
            }
        }
    }
}