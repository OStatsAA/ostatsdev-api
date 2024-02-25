using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using OStats.API.Queries;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests.Queries;

public class PeopleSearchQueryIntegrationTest : BaseIntegrationTest
{
    // https://www.name-generator.org.uk/quick/
    protected readonly List<string> GeneratedNames = new()
    {
        "Tyrese Castillo",
        "Brodie Castillo",
        "Helen Moss",
        "Helen Kline",
        "Cruz Payne",
        "Karol Welch",
        "Keziah Booker",
        "Orlando Frazier",
        "Zaara Mcconnell",
        "Kane Barnes"
    };

    public PeopleSearchQueryIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Shoud_Find_People_Matching_Anywhere()
    {
        var users = new List<User>();
        foreach (var name in GeneratedNames)
        {
            users.Add(new User(name, $"{name.Replace(" ", ".")}@test.com", $"{name.Replace(" ", "_")}_authid"));
        }
        await context.AddRangeAsync(users);
        await context.SaveChangesAsync();

        var searchResult = await UserQueries.SearchUsersAsync(context, "castil", default);
        searchResult.Should().HaveCount(2);
    }
}