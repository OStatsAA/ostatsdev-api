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
        var project = Project.Create("Test", "Test description", owner.Id, out var ownerRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(ownerRole);
        
        var editors = new[]{
            new User("Editor1", "1@test.com", "auth_id_editor1"),
            new User("Editor2", "1@test.com", "auth_id_editor2"),
        };
        await context.AddRangeAsync(editors);
        foreach (var editor in editors)
        {
            var (_, editorRole) = project.CreateUserRole(editor.Id, AccessLevel.Editor, ownerRole);
            await context.Roles.AddAsync(editorRole!);
        }
        var readers = new[]{
            new User("Reader1", "1@test.com", "auth_id_reader1"),
            new User("Reader2", "1@test.com", "auth_id_reader2"),
        };
        await context.AddRangeAsync(readers);
        foreach (var reader in readers)
        {
            var (_, readerRole) = project.CreateUserRole(reader.Id, AccessLevel.ReadOnly, ownerRole);
            await context.Roles.AddAsync(readerRole!);
        }

        await context.SaveChangesAsync();

        var queriedProjectUsersAndRoles = await ProjectQueries.GetProjectUsersAndRolesAsync(context, owner.Id, project.Id, default);

        using (new AssertionScope())
        {
            queriedProjectUsersAndRoles.Should().AllBeOfType<ProjectUserAndRoleDto>();
            queriedProjectUsersAndRoles.Should().HaveCount(1 + editors.Length + readers.Length);
        }
    }
}