using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Events;

namespace OStats.Tests.UnitTests.Domain.Aggregates.ProjectAggregate;

public class ProjectTest
{
    [Fact]
    public void Should_Be_Able_To_Add_Users_Roles()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Test Title", "Test Description", ownerId, out var ownerRole);
        var userId = Guid.NewGuid();
        var accessLevel = AccessLevel.Editor;

        var (result, userRole) = project.CreateUserRole(userId, accessLevel, ownerRole);

        result.Succeeded.Should().BeTrue();
        userRole.Should().NotBeNull();
        userRole!.AccessLevel.Should().Be(accessLevel);
        userRole.UserId.Should().Be(userId);
        userRole.ProjectId.Should().Be(project.Id);
    }

    [Fact]
    public void Should_Be_Able_To_Update_User_Role()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Test Title", "Test Description", ownerId, out var ownerRole);
        var userId = Guid.NewGuid();
        var initialAccessLevel = AccessLevel.ReadOnly;
        var updatedAccessLevel = AccessLevel.Editor;
        var (_ , userRole) = project.CreateUserRole(userId, initialAccessLevel, ownerRole);

        project.UpdateUserRole(ref userRole!, updatedAccessLevel, ownerRole);

        userRole!.AccessLevel.Should().Be(updatedAccessLevel);
    }

    [Fact]
    public void Should_Be_Able_To_Remove_User_Role()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Test Title", "Test Description", ownerId, out var ownerRole);
        var editorId = Guid.NewGuid();
        var (_, editorRole) = project.CreateUserRole(editorId, AccessLevel.Editor, ownerRole);

        project.DeleteUserRole(ref editorRole!, ownerRole);

        editorRole.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Should_Be_Able_To_Update_Project_Title()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Test Title", "Test Description", ownerId, out var ownerRole);
        var newTitle = "New Title";

        project.SetTitle(newTitle, ownerRole);

        project.Title.Should().Be(newTitle);
        project.DomainEvents.Should().ContainSingle(domainEvent => domainEvent is TitleUpdateDomainEvent);
    }

    [Fact]
    public void Should_Fail_To_Update_Project_Title_If_Requestor_Is_Not_An_Editor()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Test Title", "Test Description", ownerId, out var ownerRole);
        var (_, readOnlyUserRole) = project.CreateUserRole(Guid.NewGuid(), AccessLevel.ReadOnly, ownerRole);
        
        var newTitle = "New Title";
        var result = project.SetTitle(newTitle, readOnlyUserRole!);

        result.Succeeded.Should().BeFalse();
        project.Title.Should().NotBe(newTitle);
        project.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Should_Be_Able_To_Update_Project_Description()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Test Title", "Test Description", ownerId, out var ownerRole);
        var newDescription = "New Description";

        project.SetDescription(newDescription, ownerRole);

        project.Description.Should().Be(newDescription);
        project.DomainEvents.Should().ContainSingle(domainEvent => domainEvent is DescriptionUpdateDomainEvent);
    }

    [Fact]
    public void Should_Fail_To_Update_Project_Description_If_Requestor_Is_Not_An_Editor()
    {
        var ownerId = Guid.NewGuid();
        var project = Project.Create("Test Title", "Test Description", ownerId, out var ownerRole);
        var (_, readOnlyUserRole) = project.CreateUserRole(Guid.NewGuid(), AccessLevel.ReadOnly, ownerRole);
        
        var newDescription = "New Description";
        var result = project.SetDescription(newDescription, readOnlyUserRole!);

        result.Succeeded.Should().BeFalse();
        project.Description.Should().NotBe(newDescription);
        project.DomainEvents.Should().BeEmpty();
    }
}