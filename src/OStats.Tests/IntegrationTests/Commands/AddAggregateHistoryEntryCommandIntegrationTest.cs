using FluentAssertions.Execution;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;

namespace OStats.Tests.IntegrationTests.Commands
{
    public class AddAggregateHistoryEntryCommandIntegrationTest : BaseIntegrationTest
    {
        public AddAggregateHistoryEntryCommandIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_Add_History_Entry()
        {
            var dataset = await context.Datasets.FirstAsync();
            var consumerHarness = queueHarness.GetConsumerHarness<AddAggregateHistoryEntryCommandHandler>();

            AddAggregateHistoryEntryCommand message = new AddAggregateHistoryEntryCommand
            {
                AggregateId = dataset.Id,
                AggregateType = dataset.GetType().Name,
                UserId = Guid.NewGuid(),
                EventType = "Test",
                EventData = "Test",
                EventDescription = "Test",
                TimeStamp = DateTime.UtcNow
            };
            await queueHarness.Bus.Publish(message);

            await Task.Delay(5 * 1000);

            using (new AssertionScope())
            {
                var consumed = await consumerHarness.Consumed.SelectAsync<AddAggregateHistoryEntryCommand>().Any();
                consumed.Should().BeTrue();

                var historyEntry = await context.AggregatesHistoryEntries.Where(entry => entry.AggregateId == dataset.Id).SingleAsync();
                historyEntry.Should().NotBeNull();
                historyEntry.AggregateId.Should().Be(message.AggregateId);
                historyEntry.AggregateType.Should().Be(message.AggregateType);
                historyEntry.UserId.Should().Be(message.UserId);
                historyEntry.EventType.Should().Be(message.EventType);
                historyEntry.EventData.Should().Be(message.EventData);
                historyEntry.EventDescription.Should().Be(message.EventDescription);
            }
        }
    }
}
