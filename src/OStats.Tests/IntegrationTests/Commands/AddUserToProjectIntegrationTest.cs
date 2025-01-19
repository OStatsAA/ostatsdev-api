using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Commands;

public class AddUserToProjectIntegrationTest : BaseIntegrationTest
{
    public AddUserToProjectIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Add_User_To_Project()
    {
        var project = await context.Projects.FirstAsync();
        var ownerId = project.Roles.First(role => role.AccessLevel == AccessLevel.Owner).UserId;
        var owner = await context.Users.SingleAsync(user => user.Id == ownerId);
        var user = new User("Test", "test@test.com", "test_authid");
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        var access = AccessLevel.ReadOnly;

        var command = new AddUserToProjectCommand(owner.Id, project.Id, user.Id, access);
        var result = await serviceProvider.GetRequiredService<AddUserToProjectCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();

            project.GetUserRole(user.Id).Should().BeOfType<Role>();
            project.GetUserRole(user.Id)?.AccessLevel.Should().Be(access);
        }
    }
}