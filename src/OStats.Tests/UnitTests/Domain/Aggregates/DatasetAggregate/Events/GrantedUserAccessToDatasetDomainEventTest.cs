using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.Tests.UnitTests.Domain.Aggregates.DatasetAggregate;

public class GrantedUserAccessToDatasetDomainEventTest
{
    [Fact]
    public void GetEventDescription_WhenCalled_ReturnsCorrectDescription()
    {
        // Arrange
        var datasetUserAccessLevel = new DatasetUserAccessLevel(Guid.NewGuid(), Guid.NewGuid(), DatasetAccessLevel.Owner);
        var requestorId = Guid.NewGuid();
        var grantedUserAccessToDatasetDomainEvent = new GrantedUserAccessToDatasetDomainEvent(datasetUserAccessLevel, requestorId);
        var datasetTitle = "Test Dataset";
        var requestorName = "Test Requestor";
        var userName = "Test User";

        // Act
        var result = grantedUserAccessToDatasetDomainEvent.GetEventDescription(datasetTitle, requestorName, userName);

        // Assert
        result.Should().ContainAll(requestorName, userName, datasetUserAccessLevel.AccessLevel.ToString(), datasetTitle);
    }
}