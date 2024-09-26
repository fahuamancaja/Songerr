using System.Net;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Songerr.Application.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path != null && (
                    path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/ui/resources/", StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

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
}