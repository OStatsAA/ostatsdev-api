using FluentAssertions.Execution;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Extensions;

namespace OStats.Tests.UnitTests.Domain.Aggregates.ProjectAggregate.Extensions;

public class RolesExtensionsTest
{
    [Fact]
    public void GetUserRole_Should_Get_User_Role_By_User_Id()
    {
        var ownerId = Guid.NewGuid();
        var editorId = Guid.NewGuid();
        var project = new Project(ownerId, "Test", "Description");
        var result = project.AddOrUpdateUserRole(editorId, AccessLevel.Editor, ownerId);

        using (new AssertionScope())
        {
            result.Succeeded.Should().BeTrue();

            project.Roles.GetUserRole(ownerId)?.AccessLevel.Should().Be(AccessLevel.Owner);
            project.Roles.GetUserRole(editorId)?.AccessLevel.Should().Be(AccessLevel.Editor);
        }
    }

    [Fact]
    public void GetUsersIdsByAccessLevel_Should_Get_Users_Ids_By_Access_Level()
    {
        var ownerId = Guid.NewGuid();
        var editorsIds = new List<Guid>
        {
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()
        };
        var readOnlyIds = new List<Guid>
        {
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()
        };
        var project = new Project(ownerId, "Test", "Description");
        foreach (var editorId in editorsIds)
        {
            project.AddOrUpdateUserRole(editorId, AccessLevel.Editor, ownerId);
        }
        foreach (var readOnlyId in readOnlyIds)
        {
            project.AddOrUpdateUserRole(readOnlyId, AccessLevel.ReadOnly, ownerId);
        }

        using (new AssertionScope())
        {
            project.Roles.GetUsersIdsByAccessLevel(AccessLevel.Editor).Should().BeEquivalentTo(editorsIds);
            project.Roles.GetUsersIdsByAccessLevel(AccessLevel.ReadOnly).Should().BeEquivalentTo(readOnlyIds);
        }
    }

    [Fact]
    public void IsUser_Should_Check_If_User_Has_Access_Level()
    {
        var ownerId = Guid.NewGuid();
        var editorId = Guid.NewGuid();
        var project = new Project(ownerId, "Test", "Description");
        project.AddOrUpdateUserRole(editorId, AccessLevel.Editor, ownerId);

        using (new AssertionScope())
        {
            project.Roles.IsUser(ownerId, AccessLevel.Owner).Should().BeTrue();
            project.Roles.IsUser(ownerId, AccessLevel.Editor).Should().BeFalse();
            project.Roles.IsUser(editorId, AccessLevel.Owner).Should().BeFalse();
            project.Roles.IsUser(editorId, AccessLevel.Editor).Should().BeTrue();
        }
    }

    [Fact]
    public void IsUserAtLeast_Should_Check_If_User_Has_Minimum_Access_Level()
    {
        var ownerId = Guid.NewGuid();
        var administratorId = Guid.NewGuid();
        var editorId = Guid.NewGuid();
        var readOnlyId = Guid.NewGuid();
        var project = new Project(ownerId, "Test", "Description");
        project.AddOrUpdateUserRole(administratorId, AccessLevel.Administrator, ownerId);
        project.AddOrUpdateUserRole(editorId, AccessLevel.Editor, ownerId);
        project.AddOrUpdateUserRole(readOnlyId, AccessLevel.ReadOnly, ownerId);

        using (new AssertionScope())
        {
            project.Roles.IsUserAtLeast(ownerId, AccessLevel.Owner).Should().BeTrue();
            project.Roles.IsUserAtLeast(ownerId, AccessLevel.ReadOnly).Should().BeTrue();

            project.Roles.IsUserAtLeast(administratorId, AccessLevel.Owner).Should().BeFalse();
            project.Roles.IsUserAtLeast(administratorId, AccessLevel.Administrator).Should().BeTrue();

            project.Roles.IsUserAtLeast(editorId, AccessLevel.Administrator).Should().BeFalse();
            project.Roles.IsUserAtLeast(editorId, AccessLevel.Editor).Should().BeTrue();
            project.Roles.IsUserAtLeast(editorId, AccessLevel.ReadOnly).Should().BeTrue();

            project.Roles.IsUserAtLeast(readOnlyId, AccessLevel.Editor).Should().BeFalse();
            project.Roles.IsUserAtLeast(readOnlyId, AccessLevel.ReadOnly).Should().BeTrue();

            project.Roles.IsUserAtLeast(Guid.NewGuid(), AccessLevel.ReadOnly).Should().BeFalse();
        }
    }
}