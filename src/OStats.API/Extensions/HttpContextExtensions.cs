namespace OStats.API.Extensions;

public static class HttpContextExtensions
{
    public static string GetUserAuthId(this HttpContext context)
    {
        return context.User.Identity?.Name ?? throw new ArgumentNullException(nameof(context.User.Identity));
    }
}