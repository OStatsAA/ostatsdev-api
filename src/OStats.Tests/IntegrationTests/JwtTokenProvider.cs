using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OStats.Tests.IntegrationTests;

public static class JwtTokenProvider
{
    public static string Issuer { get; } = "Ostats";
    public static SecurityKey SecurityKey { get; } = new SymmetricSecurityKey(
        Encoding.ASCII.GetBytes("I dont like writing tests, but I dont like bugs either.")
    );
    public static SigningCredentials SigningCredentials { get; } = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    internal static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();

    public static string GenerateTokenForAuthId(string authIdentity)
    {
        var token = JwtSecurityTokenHandler.WriteToken(
            new JwtSecurityToken(
                Issuer,
                Issuer,
                [new Claim(ClaimTypes.NameIdentifier, authIdentity)],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: SigningCredentials
            )
        );

        return token;
    }

    public static string GenerateTokenForInvalidUser()
    {
        var randomString = "authid_" + Guid.NewGuid().ToString();
        var token = JwtSecurityTokenHandler.WriteToken(
            new JwtSecurityToken(
                Issuer,
                Issuer,
                [new Claim(ClaimTypes.NameIdentifier, randomString)],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: SigningCredentials
            )
        );

        return token;
    }
}