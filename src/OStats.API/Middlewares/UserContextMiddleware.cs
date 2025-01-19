using System.Text.Json;
using OStats.API.Common;

namespace OStats.API.Middlewares;

public sealed class UserContextMiddleware
{
    private readonly RequestDelegate _next;
    public static readonly string ByPassUserContextMiddleware = "ByPassUserContextMiddleware";

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserContext userContext)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<string>() == ByPassUserContextMiddleware)
        {
            await _next(context);
            return;
        }

        var user = await userContext.GetCurrentUserAsync(context.RequestAborted);
        if (user is not null)
        {
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "User not found." }));
    }
}