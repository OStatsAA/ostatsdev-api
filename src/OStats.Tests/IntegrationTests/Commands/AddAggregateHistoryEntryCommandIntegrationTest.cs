using FluentAssertions.Execution;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using OStats.API.Consumers;
using OStats.Domain.Aggregates.DatasetAggregate;

namespace OStats.Tests.IntegrationTests.Commands;

public class AddAggregateHistoryEntryCommandIntegrationTest : BaseIntegrationTest
{
    public AddAggregateHistoryEntryCommandIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Add_History_Entry()
    {
        var dataset = await context.Datasets.FirstAsync();
        var consumerHarness = queueHarness.GetConsumerHarness<GrantedUserAccessToDatasetDomainEventConsumer>();

        var message = new GrantedUserAccessToDatasetDomainEvent
        (
            new DatasetUserAccessLevel(dataset.Id, Guid.NewGuid(), DatasetAccessLevel.ReadOnly),
            Guid.NewGuid()
        );
        await queueHarness.Bus.Publish(message);

        await Task.Delay(5 * 1000);

        using (new AssertionScope())
        {
            var consumed = await consumerHarness.Consumed.SelectAsync<GrantedUserAccessToDatasetDomainEvent>().Any();
            consumed.Should().BeTrue();
        }
    }
}
