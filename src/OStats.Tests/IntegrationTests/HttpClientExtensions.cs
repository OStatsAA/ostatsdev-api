using System.Net.Http.Headers;
using OStats.Domain.Aggregates.UserAggregate;

namespace OStats.Tests.IntegrationTests;

public static class HttpClientExtensions{

    public static HttpClient WithJwtBearerTokenForUser(this HttpClient client, User user)
    {
        var token = JwtTokenProvider.GenerateTokenForAuthId(user.AuthIdentity);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static HttpClient WithJwtBearerTokenForAuthId(this HttpClient client, string authId)
    {
        var token = JwtTokenProvider.GenerateTokenForAuthId(authId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static HttpClient WithInvalidJwtBearerToken(this HttpClient client)
    {
        var token = JwtTokenProvider.GenerateTokenForInvalidUser();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}