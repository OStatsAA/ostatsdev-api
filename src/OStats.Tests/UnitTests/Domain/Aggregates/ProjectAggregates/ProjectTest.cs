using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Events;

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

    [Fact]
    public void Should_Be_Able_To_Update_Project_Title()
    {
        var ownerId = Guid.NewGuid();
        var project = new Project(ownerId, "Test Title", "Test Description");
        var newTitle = "New Title";

        project.SetTitle(newTitle, project.GetUserRole(ownerId)!);

        project.Title.Should().Be(newTitle);
        project.DomainEvents.Should().ContainSingle(domainEvent => domainEvent is TitleUpdate);
    }

    [Fact]
    public void Should_Fail_To_Update_Project_Title_If_Requestor_Is_Not_An_Editor()
    {
        var ownerId = Guid.NewGuid();
        var project = new Project(ownerId, "Test Title", "Test Description");
        var newTitle = "New Title";

        var result = project.SetTitle(newTitle, new Role(ownerId, ownerId, AccessLevel.ReadOnly));

        result.Succeeded.Should().BeFalse();
        project.Title.Should().NotBe(newTitle);
        project.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Should_Be_Able_To_Update_Project_Description()
    {
        var ownerId = Guid.NewGuid();
        var project = new Project(ownerId, "Test Title", "Test Description");
        var newDescription = "New Description";

        project.SetDescription(newDescription, project.GetUserRole(ownerId)!);

        project.Description.Should().Be(newDescription);
        project.DomainEvents.Should().ContainSingle(domainEvent => domainEvent is DescriptionUpdate);
    }

    [Fact]
    public void Should_Fail_To_Update_Project_Description_If_Requestor_Is_Not_An_Editor()
    {
        var ownerId = Guid.NewGuid();
        var project = new Project(ownerId, "Test Title", "Test Description");
        var newDescription = "New Description";

        var result = project.SetDescription(newDescription, new Role(ownerId, ownerId, AccessLevel.ReadOnly));

        result.Succeeded.Should().BeFalse();
        project.Description.Should().NotBe(newDescription);
        project.DomainEvents.Should().BeEmpty();
    }
}