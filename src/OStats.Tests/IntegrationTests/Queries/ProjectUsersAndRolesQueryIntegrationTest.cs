using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.API.Queries;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class ProjectUsersAndRolesQueryIntegrationTest : BaseIntegrationTest
{
    public ProjectUsersAndRolesQueryIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Get_Project_Users_And_Roles()
    {
        var owner = await context.Users.FirstAsync();
        var project = new Project(owner.Id, "Test", "Test description");
        await context.Projects.AddAsync(project);
        var editors = new[]{
            new User("Editor1", "1@test.com", "auth_id_editor1"),
            new User("Editor2", "1@test.com", "auth_id_editor2"),
        };
        await context.AddRangeAsync(editors);
        foreach (var editor in editors)
        {
            project.AddOrUpdateUserRole(editor.Id, AccessLevel.Editor);
        }
        var readers = new[]{
            new User("Reader1", "1@test.com", "auth_id_reader1"),
            new User("Reader2", "1@test.com", "auth_id_reader2"),
        };
        await context.AddRangeAsync(readers);
        foreach (var reader in readers)
        {
            project.AddOrUpdateUserRole(reader.Id, AccessLevel.ReadOnly);
        };

        await context.SaveChangesAsync();

        var query = new ProjectUsersAndRolesQuery(owner.AuthIdentity, project.Id);
        var result = await sender.Send(query);

        using (new AssertionScope())
        {
            result.Value.Should().AllBeOfType<ProjectUserAndRoleDto>();
            result.Value.Should().HaveCount(1 + editors.Length + readers.Length);
        }
    }
}