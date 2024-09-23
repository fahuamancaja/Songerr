using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Songerr.Application.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = GetStatusCode(exception);
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Title = exception.Message,
            Status = statusCode,
            Detail = exception.StackTrace,
        };
        
        Log.Error(exception, "An error occurred.");
        
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails.ToString());

        return true;
    }

    private int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            FileNotFoundException => StatusCodes.Status404NotFound,
            NullReferenceException => StatusCodes.Status500InternalServerError,
            DivideByZeroException => StatusCodes.Status500InternalServerError,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}