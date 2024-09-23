using System.Net;

namespace Songerr.Application;

public class ApiKeyMiddleware
{
    private readonly IConfiguration _config;
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Request.Headers["X-API-Key"]))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        string? userApiKey = context.Request.Headers["X-API-Key"];
        var validApiKey = _config["ApiKeys:MyApiKey"];

        if (userApiKey != validApiKey)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        await _next(context);
    }
}