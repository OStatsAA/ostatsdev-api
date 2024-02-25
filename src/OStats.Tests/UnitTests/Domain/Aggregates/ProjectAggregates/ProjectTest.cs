using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Tests.UnitTests.Domain.Aggregates.ProjectAggregate;

public class ProjectTest
{
    [Fact]
    public void Should_Be_Able_To_Add_Users_Roles()
    {
        var ownerId = Guid.NewGuid();
        var project = new Project(ownerId, "Test Title", "Test Description");
        var userId = Guid.NewGuid();
        var accessLevel = AccessLevel.Editor;

        project.AddOrUpdateUserRole(userId, accessLevel, ownerId);

        project.Roles
            .Single(role => role.UserId == userId && role.AccessLevel == accessLevel)
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Should_Be_Able_To_Update_User_Role()
    {
        var ownerId = Guid.NewGuid();
        var project = new Project(ownerId, "Test Title", "Test Description");
        var userId = Guid.NewGuid();
        var initialAccessLevel = AccessLevel.ReadOnly;
        var updatedAccessLevel = AccessLevel.Editor;
        project.AddOrUpdateUserRole(userId, initialAccessLevel, ownerId);
        project.AddOrUpdateUserRole(userId, updatedAccessLevel, ownerId);

        project.Roles.Should().NotContain(role => role.UserId == userId
                                                  && role.AccessLevel == initialAccessLevel);

        project.Roles
            .Should()
            .ContainSingle(role => role.UserId == userId
                                   && role.AccessLevel == updatedAccessLevel);
    }

    [Fact]
    public void Should_Be_Able_To_Remove_User_Role()
    {
        var ownerId = Guid.NewGuid();
        var project = new Project(ownerId, "Test Title", "Test Description");
        var editorId = Guid.NewGuid();
        project.AddOrUpdateUserRole(editorId, AccessLevel.Editor, ownerId);
        project.Roles.Should().HaveCount(2);

        project.RemoveUserRole(editorId, ownerId);

        project.Roles.Should().HaveCount(1);
        project.Roles.Should().NotContain(role => role.UserId == editorId);
    }

    [Fact]
    public void Should_Be_Able_To_Get_User_Role_By_Id()
    {
        var ownerAccessLevel = AccessLevel.Owner;
        var ownerId = Guid.NewGuid();
        var project = new Project(ownerId, "Test Title", "Test Description");
        var randomGuid = Guid.NewGuid();

        project.GetUserRole(ownerId)
               .Should()
               .Match<Role>(role => role.AccessLevel == ownerAccessLevel);
        project.GetUserRole(randomGuid)
               .Should()
               .BeNull();
    }
}