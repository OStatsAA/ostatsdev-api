using System.Security.Claims;

namespace OStats.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetAuthId(this ClaimsPrincipal claims)
    {
        return claims.Identity?.Name ?? throw new ArgumentNullException(nameof(claims.Identity));
    }
}