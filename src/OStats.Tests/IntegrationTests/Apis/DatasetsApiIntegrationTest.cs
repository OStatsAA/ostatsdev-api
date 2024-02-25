using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;
using OStats.Domain.Aggregates.DatasetAggregate;
using OStats.Domain.Aggregates.ProjectAggregate;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Apis;

public class DatasetsApiIntegrationTest : BaseIntegrationTest
{
    private readonly string _baseUrl = "/v1/datasets";

    public DatasetsApiIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    public static IEnumerable<object[]> InvalidCreateDatasetDtos()
    {
        return new List<object[]>
        {
            new object[]
            {
                new CreateDatasetDto { Title = "", Source = "Source", Description = "Description" }
            },
            new object[]
            {
                new CreateDatasetDto { Title = "Title", Source = "", Description = "Description" }
            },
            new object[]
            {
                new CreateDatasetDto { Title = new string('a', 257), Source = "Source", Description = "Description" }
            },
            new object[]
            {
                new CreateDatasetDto { Title = "Title", Source = new string('a', 257), Description = "Description" }
            },
            new object[]
            {
                new CreateDatasetDto { Title = "Title", Source = "Source", Description = new string('a', 4097) }
            }
        };
    }

    [Fact]
    public async Task Should_Create_Dataset()
    {
        var createDto = new CreateDatasetDto
        {
            Title = "Test Dataset",
            Source = "Test Source",
            Description = "Test Description"
        };

        var token = JwtTokenProvider.GenerateTokenForAuthId("test_auth_identity");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(_baseUrl, createDto);

        using (new AssertionScope())
        {
            var baseDataset = await response.Content.ReadFromJsonAsync<BaseDatasetDto>();

            baseDataset.Should().NotBeNull();
            baseDataset!.Id.Should().NotBeEmpty();
            baseDataset!.Title.Should().Be(createDto.Title);
            baseDataset.Source.Should().Be(createDto.Source);
            baseDataset.Description.Should().Be(createDto.Description);

            var isCreated = await context.Datasets.AnyAsync(d => d.Id == baseDataset.Id);
            isCreated.Should().BeTrue();
        }
    }

    [Theory]
    [MemberData(nameof(InvalidCreateDatasetDtos))]
    public async Task Should_Not_Create_Dataset_With_Invalid_Input(CreateDatasetDto createDto)
    {
        var validUser = await context.Users.FirstAsync();
        var validUserProjectsCount = await context.Roles.Where(r => r.UserId == validUser.Id).CountAsync();
        context.ChangeTracker.Clear();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(_baseUrl, createDto);

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeFalse();

            var afterCallUserProjectsCount = await context.Roles.Where(r => r.UserId == validUser.Id).CountAsync();
            afterCallUserProjectsCount.Should().Be(validUserProjectsCount);
        }
    }


    //TODO: get dataset by id

    [Fact]
    public async Task Should_Delete_Dataset()
    {
        var validUser = await context.Users.FirstAsync();
        var dataset = new Dataset(validUser.Id, "Test Dataset", "Test Source", "Test Description");
        await context.Datasets.AddAsync(dataset);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync($"{_baseUrl}/{dataset.Id}");

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            var isDeleted = await context.Datasets.AnyAsync(d => d.Id == dataset.Id);
            isDeleted.Should().BeFalse();
        }
    }

    [Fact]
    public async Task Should_Update_Dataset()
    {
        var validUser = await context.Users.FirstAsync();
        var dataset = new Dataset(validUser.Id, "Test Dataset", "Test Source", "Test Description");
        await context.Datasets.AddAsync(dataset);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var updateDto = new UpdateDatasetDto
        {
            Title = "Updated Test Dataset",
            Source = "Updated Test Source",
            Description = "Updated Test Description",
            LastUpdatedAt = dataset.LastUpdatedAt
        };

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsJsonAsync($"{_baseUrl}/{dataset.Id}", updateDto);
        var baseDataset = await response.Content.ReadFromJsonAsync<BaseDatasetDto>();

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            baseDataset.Should().NotBeNull();
            baseDataset!.Id.Should().Be(dataset.Id);
            baseDataset!.Title.Should().Be(updateDto.Title);
            baseDataset.Source.Should().Be(updateDto.Source);
            baseDataset.Description.Should().Be(updateDto.Description);

            var updatedDataset = await context.Datasets.FindAsync(dataset.Id);
            updatedDataset!.Title.Should().Be(updateDto.Title);
            updatedDataset!.Source.Should().Be(updateDto.Source);
            updatedDataset!.Description.Should().Be(updateDto.Description);
        }
    }

    [Fact]
    public async Task Should_Add_User_Access_To_Dataset()
    {
        var validUser = await context.Users.FirstAsync();
        var dataset = new Dataset(validUser.Id, "Test Dataset", "Test Source", "Test Description");
        await context.Datasets.AddAsync(dataset);

        var other = new User("Other User", "other@other.com", "other_auth_identity");
        await context.Users.AddAsync(other);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            $"{_baseUrl}/{dataset.Id}/users",
            new AddUserToDatasetDto { UserId = other.Id, AccessLevel = DatasetAccessLevel.ReadOnly });

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            var datasetAccessLevel = await context.DatasetsUsersAccessLevels
                .Where(datasetAccessLevel => datasetAccessLevel.DatasetId == dataset.Id &&
                                             datasetAccessLevel.UserId == other.Id)
                .SingleOrDefaultAsync();

            datasetAccessLevel.Should().NotBeNull();
            datasetAccessLevel!.AccessLevel.Should().Be(DatasetAccessLevel.ReadOnly);
        }
    }

    [Fact]
    public async Task Should_Remove_User_Access_From_Dataset()
    {
        var validUser = await context.Users.FirstAsync();
        var dataset = new Dataset(validUser.Id, "Test Dataset", "Test Source", "Test Description");
        await context.Datasets.AddAsync(dataset);

        var other = new User("Other User TBD", "other_tbd@other.com", "other_auth_identity_tbd");
        await context.Users.AddAsync(other);

        dataset.GrantUserAccess(other.Id, DatasetAccessLevel.ReadOnly, validUser.Id);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync($"{_baseUrl}/{dataset.Id}/users/{other.Id}");

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();

            var hasAccess = await context.DatasetsUsersAccessLevels
                .Where(datasetAccessLevel => datasetAccessLevel.DatasetId == dataset.Id &&
                                             datasetAccessLevel.UserId == other.Id)
                .AnyAsync();

            hasAccess.Should().BeFalse();
        }
    }

    [Fact]
    public async Task Should_Get_Dataset_Linked_Projects()
    {
        var validUser = await context.Users.FirstAsync();
        var dataset = new Dataset(validUser.Id, "Test Dataset", "Source");
        await context.Datasets.AddAsync(dataset);
        var project = new Project(validUser.Id, "Test Project");
        await context.Projects.AddAsync(project);
        project.LinkDataset(dataset.Id, validUser.Id);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"{_baseUrl}/{dataset.Id}/linkedprojects");
        var links = await response.Content.ReadFromJsonAsync<List<object>>();

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            links.Should().NotBeNullOrEmpty();
            links.Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task Should_Fail_To_Query_If_User_Is_Not_Valid()
    {
        var dataset = await context.Datasets.FirstAsync();
        var token = JwtTokenProvider.GenerateTokenForInvalidUser();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"{_baseUrl}/{dataset.Id}/data");

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}