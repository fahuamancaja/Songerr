using System.Diagnostics.CodeAnalysis;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Songerr.Application.Middleware;

namespace Songerr.Application;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Serilog Configuration
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration) // Read from configuration
            .CreateLogger();

        // Register services by calling the RegisterServices method
        builder.Services.RegisterServices(builder.Configuration);

        var app = builder.Build();

        // Enable the Middleware for development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Enable static files middleware
        app.UseStaticFiles();

        app.UseRouting();

        // Add TraceIdMiddleware to the pipeline
        app.UseMiddleware<TraceIdMiddleware>();

        // API Key Middleware
        app.UseMiddleware<ApiKeyMiddleware>();

        // Middleware Healthchecks
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecksUI(setup => { setup.AddCustomStylesheet("wwwroot/Assets/dotnet.css"); });

        // Global exception handler
        var globalExceptionHandler = new GlobalExceptionHandler();
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature?.Error != null)
                {
                    await globalExceptionHandler.TryHandleAsync(context, exceptionHandlerFeature.Error, new CancellationToken());
                }
            });
        });


        // Map other endpoints, including controllers
        app.MapControllers();

        app.Run();
    }
}