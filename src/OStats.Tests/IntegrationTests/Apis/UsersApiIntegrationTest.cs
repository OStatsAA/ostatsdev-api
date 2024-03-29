using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Dtos;

namespace OStats.Tests.IntegrationTests.Apis;

public class UsersApiIntegrationTest : BaseIntegrationTest
{
    private readonly string _base_url = "/v1/users";

    public UsersApiIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    public static IEnumerable<object[]> InvalidCreateUserDtos()
    {
        return new List<object[]>
        {
            new object[] { new CreateUserDto { Name = "", Email = "email@email.com"} },
            new object[] { new CreateUserDto { Name = "Name", Email = ""} },
            new object[] { new CreateUserDto { Name = new string('a', 129), Email = "email@email.com"} },
            new object[] { new CreateUserDto { Name = "Name", Email = "email@" + new string('a', 129)} },
            new object[] { new CreateUserDto { Name = "Name", Email =  new string('a', 129)} },
            new object[] { new CreateUserDto { Name = "Name", Email = "invalid_email"} }
        };
    }


    [Fact]
    public async Task Should_Search_Users()
    {
        var validUser = await context.Users.FirstAsync();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"{_base_url}?search={validUser.Name}");
        var results = await response.Content.ReadFromJsonAsync<List<BaseUserDto>>();

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            results.Should().NotBeNullOrEmpty();
            results.Should().ContainSingle(u => u.Id == validUser.Id);
        }
    }

    [Fact]
    public async Task Should_Get_User_By_Id()
    {
        var validUser = await context.Users.FirstAsync();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"{_base_url}/{validUser.Id}");
        var result = await response.Content.ReadFromJsonAsync<BaseUserDto>();

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            result.Should().NotBeNull();
            result!.Id.Should().Be(validUser.Id);
        }
    }

    [Fact]
    public async Task Should_Create_User()
    {
        var userDto = new CreateUserDto
        {
            Name = "Test User Created",
            Email = "test@test.com"
        };

        var token = JwtTokenProvider.GenerateTokenForAuthId("test|user_create_test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(_base_url, userDto);
        var result = await response.Content.ReadFromJsonAsync<BaseUserDto>();

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            result.Should().NotBeNull();
            result!.Name.Should().Be(userDto.Name);
            result!.Email.Should().Be(userDto.Email);
        }
    }

    [Theory]
    [MemberData(nameof(InvalidCreateUserDtos))]
    public async Task Should_Not_Create_User_With_Invalid_Input(CreateUserDto userDto)
    {
        var validUser = await context.Users.FirstAsync();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(_base_url, userDto);

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }

    [Fact]
    public async Task Should_Get_User_Projects()
    {
        var validUser = await context.Users.FirstAsync();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"{_base_url}/{validUser.Id}/projects");
        var userProjects = await response.Content.ReadFromJsonAsync<IEnumerable<object>>();

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            userProjects.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task Should_Get_User_Datasets()
    {
        var validUser = await context.Users.FirstAsync();

        var token = JwtTokenProvider.GenerateTokenForAuthId(validUser.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"{_base_url}/{validUser.Id}/datasets");
        var userDatasets = await response.Content.ReadFromJsonAsync<IEnumerable<object>>();

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            userDatasets.Should().NotBeNullOrEmpty();
        }
    }
}