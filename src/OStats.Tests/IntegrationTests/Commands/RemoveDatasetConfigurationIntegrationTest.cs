using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Tests.IntegrationTests.Commands;

public class RemoveDatasetConfigurationIntegrationTest : BaseIntegrationTest
{
    public RemoveDatasetConfigurationIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Remove_Dataset_Configuration_From_Project()
    {
        var existingUser = await context.Users.FirstAsync();
        var project = new Project(existingUser.Id, "Test", "Test description");
        var datasetConfig = new DatasetConfiguration("Test", "Test");
        project.AddDatasetConfiguration(datasetConfig);
        context.Add(project);
        context.SaveChanges();

        var command = new RemoveDatasetConfigurationCommand(existingUser.AuthIdentity, project.Id, datasetConfig.Id);
        var result = await sender.Send(command);

        using (new AssertionScope())
        {
            result.Success.Should().BeTrue();
            result.ValidationFailures.Should().BeNull();
            project.DatasetsConfigs.Any(dsConfig => dsConfig.Id == datasetConfig.Id).Should().BeFalse();
            context.Projects.Where(p => p.Id == project.Id).Any().Should().BeTrue();
            context.DatasetsConfigurations.Where(dsConfig => dsConfig.Id == datasetConfig.Id).Any().Should().BeFalse();
        }
    }
}