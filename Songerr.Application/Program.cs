using System.Diagnostics.CodeAnalysis;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Sinks.Elasticsearch;
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

        app.UseMiddleware<ApiKeyMiddleware>();
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        
        app.UseAuthorization();

        app.UseEndpoints(config => config.MapHealthChecksUI(setup =>
        {
            setup.AddCustomStylesheet("wwwroot/Assets/dotnet.css");
        }));

        // Global exception handler
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature?.Error != null)
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = exceptionHandlerFeature.Error.Message
                    });
            });
        });


        // Map other endpoints, including controllers
        app.MapControllers();

        app.Run();
    }
}