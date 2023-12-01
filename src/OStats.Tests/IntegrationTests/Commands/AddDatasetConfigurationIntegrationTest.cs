using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Commands;

public class AddDatasetConfigurationIntegrationTest : BaseIntegrationTest
{
    public AddDatasetConfigurationIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Add_Dataset_Configuration_To_Project()
    {
        var existingUser = await context.Users.FirstAsync();
        var project = new Project(existingUser.Id, "Test", "Test description");
        context.Projects.Add(project);
        context.SaveChanges();

        var command = new AddDatasetConfigurationCommand(
            existingUser.AuthIdentity,
            project.Id,
            "Test DatasetConfiguration",
            "Test source",
            "Test description"
        );

        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.ValidationFailures.Should().BeNull();
            result.Value.Should().NotBeNull().And.BeOfType<DatasetConfiguration>();
            result.Value?.ProjectId.Should().Be(project.Id);
        }
    }

    [Fact]
    public async Task Should_Fail_If_User_Is_Not_At_Least_Editor()
    {
        var project = await context.Projects.FirstAsync();
        var readOnlyUser = new User("ReadOnly", "readonly@test.com", "read_only_auth_id");
        context.Add(readOnlyUser);
        project.AddOrUpdateUserRole(readOnlyUser.Id, AccessLevel.ReadOnly);
        await context.SaveChangesAsync();

        var command = new AddDatasetConfigurationCommand(
            readOnlyUser.AuthIdentity,
            project.Id,
            "Test DatasetConfiguration",
            "Test source",
            "Test description"
        );

        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Success.Should().BeFalse();
            result.ValidationFailures.Should().NotBeNullOrEmpty();
            result.Value.Should().BeNull();
        }
    }
}