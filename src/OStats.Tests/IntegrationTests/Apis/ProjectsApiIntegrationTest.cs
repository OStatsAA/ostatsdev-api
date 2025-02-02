using System.Net;
using System.Net.Http.Json;
using FluentAssertions.Execution;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.ProjectAggregate.Events;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Apis;

public class ProjectsApiIntegrationTest : BaseIntegrationTest
{
    private readonly string _base_url = "/v1/projects";

    public ProjectsApiIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    public static IEnumerable<object[]> InvalidCreateProjectDtos()
    {
        return
        [
            [new CreateProjectDto { Title = "" }],
            [new CreateProjectDto { Title = new string('a', 257) }],
        ];
    }

    [Fact]
    public async Task Should_Create_Project()
    {
        var response = await client
            .WithJwtBearerTokenForUser(await context.Users.FirstAsync())
            .PostAsJsonAsync(_base_url, new CreateProjectDto { Title = "Test Project" });

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            var baseProject = await response.Content.ReadFromJsonAsync<BaseProjectDto>();
            baseProject.Should().NotBeNull();
            baseProject!.Id.Should().NotBeEmpty();
            baseProject!.Title.Should().Be("Test Project");
            baseProject!.Description.Should().BeNullOrEmpty();
            baseProject!.CreatedAt.Should().BeBefore(DateTime.Now);
            baseProject!.LastUpdatedAt.Should().BeBefore(DateTime.Now);

            var project = await context.Projects.FindAsync(baseProject.Id);
            project.Should().NotBeNull();
        }
    }

    [Theory]
    [MemberData(nameof(InvalidCreateProjectDtos))]
    public async Task Should_Not_Create_With_Invalid_Input(CreateProjectDto createProjectDto)
    {
        var response = await client
            .WithJwtBearerTokenForUser(await context.Users.FirstAsync())
            .PostAsJsonAsync(_base_url, createProjectDto);

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }

    [Fact]
    public async Task Should_Fail_If_Invalid_User()
    {
        var response = await client
            .WithInvalidJwtBearerToken()
            .PostAsJsonAsync(_base_url, new CreateProjectDto { Title = "Test Project" });

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeFalse();
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("User not found");
        }
    }

    [Fact]
    public async Task Should_Get_Project_By_Id()
    {
        var validUser = await context.Users.FirstAsync();
        var project = Project.Create("Test Project", "Test Description", validUser.Id, out var requestorRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(requestorRole);
        await context.SaveChangesAsync();

        var response = await client
            .WithJwtBearerTokenForUser(validUser)
            .GetAsync($"{_base_url}/{project.Id}");

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            var projectDto = await response.Content.ReadFromJsonAsync<ProjectDto>();
            projectDto.Should().NotBeNull();
            projectDto!.Id.Should().Be(project.Id);
        }
    }

    [Fact]
    public async Task Should_Fail_If_Not_Found()
    {
        var validUser = await context.Users.FirstAsync();

        var response = await client
            .WithJwtBearerTokenForUser(validUser)
            .GetAsync($"{_base_url}/{Guid.NewGuid()}");

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task Should_Update_Project()
    {
        var validUser = await context.Users.FirstAsync();
        var project = Project.Create("Test Project", "Test Description", validUser.Id, out var requestorRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(requestorRole);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var response = await client
            .WithJwtBearerTokenForUser(validUser)
            .PutAsJsonAsync($"{_base_url}/{project.Id}", new UpdateProjectDto
            {
                Title = "Updated Project",
                Description = "Updated Description",
                LastUpdatedAt = project.LastUpdatedAt
            });

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            var baseProject = await response.Content.ReadFromJsonAsync<BaseProjectDto>();
            baseProject.Should().NotBeNull();
            baseProject!.Id.Should().Be(project.Id);
            baseProject!.Title.Should().Be("Updated Project");
            baseProject!.Description.Should().Be("Updated Description");
            baseProject!.LastUpdatedAt.Should().BeAfter(project.LastUpdatedAt);

            var updatedProject = await context.Projects.FindAsync(project.Id);
            updatedProject!.Title.Should().Be("Updated Project");
            updatedProject!.Description.Should().Be("Updated Description");
            updatedProject!.LastUpdatedAt.Should().BeAfter(project.LastUpdatedAt);
        }
    }

    [Fact]
    public async Task Should_Fail_If_Update_With_Invalid_Last_Updated_At()
    {
        var validUser = await context.Users.FirstAsync();
        var project = Project.Create("Test Project", "Test Description", validUser.Id, out var requestorRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(requestorRole);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var response = await client
            .WithJwtBearerTokenForUser(validUser)
            .PutAsJsonAsync($"{_base_url}/{project.Id}", new UpdateProjectDto
            {
                Title = "Updated Project",
                Description = "Updated Description",
                LastUpdatedAt = DateTime.Now.AddMinutes(-5)
            });

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeFalse();
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Contain("Project has changed");

            var unmodifiedProject = await context.Projects.FindAsync(project.Id);
            unmodifiedProject!.Title.Should().Be("Test Project");
            unmodifiedProject!.Description.Should().Be("Test Description");
        }
    }

    [Fact]
    public async Task Should_Delete_Project()
    {
        var validUser = await context.Users.FirstAsync();
        var project = Project.Create("Test Project", "Test Description", validUser.Id, out var requestorRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(requestorRole);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var response = await client
            .WithJwtBearerTokenForUser(validUser)
            .DeleteAsync($"{_base_url}/{project.Id}");

        await Task.Delay(5 * 1000);

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            var isDeleted = !await context.Projects.AnyAsync(p => p.Id == project.Id);
            isDeleted.Should().BeTrue();

            var _event = await queueHarness.Published.SelectAsync<DeletedProjectDomainEvent>(_event => _event.Context.Message.ProjectId == project.Id).FirstOrDefault();
            _event.Should().NotBeNull();
            var consumed = await queueHarness.Consumed.Any<DeletedProjectDomainEvent>(_event => _event.Context.Message.ProjectId == project.Id);
            consumed.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Should_Link_Datasets_To_Project()
    {
        var validUser = await context.Users.FirstAsync();
        var project = Project.Create("Test Project", "Test Description", validUser.Id, out var requestorRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(requestorRole);
        var dataset = new Dataset(validUser.Id, "Test Dataset", "Test Description");
        await context.Datasets.AddAsync(dataset);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var response = await client
            .WithJwtBearerTokenForUser(validUser)
            .PostAsync($"{_base_url}/{project.Id}/linkdataset/{dataset.Id}", null);

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            var linkedDatasets = await context.DatasetsProjectsLinks
                .Where(dpLink => dpLink.ProjectId == project.Id)
                .Select(dpLink => dpLink.DatasetId)
                .ToListAsync();

            linkedDatasets.Should().Contain(dataset.Id);
        }
    }

    [Fact]
    public async Task Should_Unlink_Datasets_From_Project()
    {
        var validUser = await context.Users.FirstAsync();
        var project = Project.Create("Test Project", "Test Description", validUser.Id, out var requestorRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(requestorRole);
        var dataset = new Dataset(validUser.Id, "Test Dataset", "Test Description");
        await context.Datasets.AddAsync(dataset);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var link = new DatasetProjectLink(dataset.Id, project.Id);
        await context.DatasetsProjectsLinks.AddAsync(link);
        await context.SaveChangesAsync();

        var response = await client
            .WithJwtBearerTokenForUser(validUser)
            .DeleteAsync($"{_base_url}/{project.Id}/linkdataset/{dataset.Id}");

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            var linkedDatasets = await context.DatasetsProjectsLinks
                .Where(dpLink => dpLink.ProjectId == project.Id)
                .Select(dpLink => dpLink.DatasetId)
                .ToListAsync();

            linkedDatasets.Should().NotContain(dataset.Id);
        }
    }

    [Fact]
    public async Task Should_Get_Project_Users_And_Roles()
    {
        var validUser = await context.Users.FirstAsync();
        var project = Project.Create("Test Project", "Test Description", validUser.Id, out var requestorRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(requestorRole);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var response = await client
            .WithJwtBearerTokenForUser(validUser)
            .GetAsync($"{_base_url}/{project.Id}/users");

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            var usersAndRoles = await response.Content.ReadFromJsonAsync<List<ProjectUserAndRoleDto>>();
            usersAndRoles.Should().NotBeNull();
            usersAndRoles!.Should().ContainSingle();
            usersAndRoles!.Single().User.Id.Should().Be(validUser.Id);
        }
    }

    [Fact]
    public async Task Should_Add_User_To_Project()
    {
        var owner = await context.Users.FirstAsync();
        var project = Project.Create("Test Project", "Test Description", owner.Id, out var requestorRole);
        await context.Projects.AddAsync(project);
        await context.Roles.AddAsync(requestorRole);
        var other = new User("Other User", "other@other.com", "other_auth_identity");
        await context.Users.AddAsync(other);
        
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var response = await client
            .WithJwtBearerTokenForUser(owner)
            .PostAsJsonAsync(
            $"{_base_url}/{project.Id}/users",
            new AddUserToProjectDto { UserId = other.Id, AccessLevel = AccessLevel.Editor });

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            var otherUserRole = await context.Roles
                .FirstOrDefaultAsync(role => role.ProjectId == project.Id && role.UserId == other.Id);

            otherUserRole.Should().NotBeNull();
            otherUserRole!.AccessLevel.Should().Be(AccessLevel.Editor);
        }
    }

    [Fact]
    public async Task Should_Remove_User_From_Project()
    {
        var owner = await context.Users.FirstAsync();
        var project = Project.Create("Test Project", "Test Description", owner.Id, out var ownerRole);
        await context.AddAsync(project);
        await context.AddAsync(ownerRole);

        var other = new User("Other User TBD", "other@other.com", "other_auth_identity_tbd");
        await context.Users.AddAsync(other);

        var (_, otherUserRole) = project.CreateUserRole(other.Id, AccessLevel.Editor, ownerRole);
        await context.AddAsync(otherUserRole!);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var response = await client
            .WithJwtBearerTokenForUser(owner)
            .DeleteAsync($"{_base_url}/{project.Id}/users/{other.Id}");

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            var persistedRole = await context.Roles
                .FirstOrDefaultAsync(role => role.ProjectId == project.Id && role.UserId == other.Id);

            persistedRole.Should().BeNull();
        }
    }
}