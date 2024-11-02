using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.Tests.UnitTests.Domain.Aggregates.DatasetAggregate;

public class DatasetTests
{
    [Fact]
    public void Should_Be_Able_To_Add_Users_Roles()
    {
        var ownerId = Guid.NewGuid();
        var dataset = new Dataset(ownerId, "Test Title", "Test Description");
        var userId = Guid.NewGuid();
        var accessLevel = DatasetAccessLevel.Editor;

        var result = dataset.GrantUserAccess(userId, accessLevel, ownerId);

        result.Succeeded.Should().BeTrue();
        dataset.DatasetUserAccessLevels
            .Single(role => role.UserId == userId && role.AccessLevel == accessLevel)
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Should_Be_Able_To_Update_Dataset_Visibility()
    {
        var ownerId = Guid.NewGuid();
        var dataset = new Dataset(ownerId, "Test Title", "Test Description");
        var isPublic = !dataset.IsPublic;

        var result = dataset.UpdateVisibility(ownerId, isPublic);

        result.Succeeded.Should().BeTrue();
        dataset.IsPublic.Should().Be(isPublic);
    }

    [Fact]
    public void Should_Not_Be_Able_To_Update_Dataset_Visibility_If_Not_At_Least_Administrator()
    {
        var ownerId = Guid.NewGuid();
        var dataset = new Dataset(ownerId, "Test Title", "Test Description");
        var isPublic = !dataset.IsPublic;

        var result = dataset.UpdateVisibility(Guid.NewGuid(), isPublic);

        result.Succeeded.Should().BeFalse();
        dataset.IsPublic.Should().Be(!isPublic);
    }

    [Fact]
    public void Should_Take_No_Action_If_Visibility_Already_Set()
    {
        var ownerId = Guid.NewGuid();
        var dataset = new Dataset(ownerId, "Test Title", "Test Description");
        var isPublic = dataset.IsPublic;

        var result = dataset.UpdateVisibility(ownerId, isPublic);

        result.Succeeded.Should().BeFalse();
        dataset.IsPublic.Should().Be(isPublic);
    }
}