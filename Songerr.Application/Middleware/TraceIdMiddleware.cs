using System.Diagnostics;
using Serilog.Context;

namespace Songerr.Application.Middleware;

public class TraceIdMiddleware
{
    private readonly RequestDelegate _next;

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        using (LogContext.PushProperty("TraceId", traceId))
        {
            await _next(context);
        }
    }
}