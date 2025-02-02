using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;
using OStats.Infrastructure;

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
        var ownerRole = await context.Roles.FindProjectOwnerAsync(project.Id, default);
        var user = new User("Test", "test@test.com", "test_authid");
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        var access = AccessLevel.ReadOnly;

        var command = new AddUserToProjectCommand(ownerRole.UserId, project.Id, user.Id, access);
        var result = await serviceProvider.GetRequiredService<AddUserToProjectCommandHandler>().Handle(command, default);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();

            var userRole = await context.Roles.FindByProjectIdAndUserIdAsync(project.Id, user.Id, default);
            userRole.Should().NotBeNull();
            userRole!.AccessLevel.Should().Be(access);
        }
    }
}