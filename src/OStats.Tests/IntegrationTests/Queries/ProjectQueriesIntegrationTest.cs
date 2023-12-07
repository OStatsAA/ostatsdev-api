using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Commands;
using OStats.Domain.Aggregates.ProjectAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class ProjectQueriesIntegrationTest : BaseIntegrationTest
{
    public ProjectQueriesIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Get_Project_By_Id()
    {
        var existingUser = await context.Users.FirstAsync();
        var project = new Project(existingUser.Id, "Test", "Test description");
        var datasetConfig = new DatasetConfiguration("Test DatasetConfiguration",
            "Test source",
            "Test description");
        project.AddDatasetConfiguration(datasetConfig);
        context.Projects.Add(project);
        context.SaveChanges();

        var queriedProject = await projectQueries.GetProjectByIdAsync(project.Id);

        using (new AssertionScope())
        {
            queriedProject.Should().NotBeNull();
        }
    }
}