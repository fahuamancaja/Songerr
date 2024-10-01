using System.Diagnostics;
using Serilog.Context;

namespace Songerr.Application.Middleware;

public class TraceIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        using (LogContext.PushProperty("TraceId", traceId))
        {
            await next(context);
        }
    }
}