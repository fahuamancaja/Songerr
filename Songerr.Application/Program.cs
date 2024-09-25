using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Songerr.Application.Middleware;

namespace Songerr.Application;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Serilog Configuration
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
            {
                IndexFormat = "aspnetcore-logs-{0:yyyy.MM.dd}",
                AutoRegisterTemplate = true,
                NumberOfShards = 2,
                NumberOfReplicas = 1
            })
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Register services by calling the RegisterServices method
        builder.Services.RegisterServices(builder.Configuration);
        builder.Services.AddHealthChecks();

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

        // Apply custom middleware only to non-health endpoints
        app.UseWhen(context => !context.Request.Path.StartsWithSegments("/health"),
            appBuilder => { appBuilder.UseMiddleware<ApiKeyMiddleware>(); });

        // Map health check endpoints and ensure they return JSON
        app.MapHealthChecks("/health");

        // Global exception handler
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature?.Error != null)
                    await context.Response.WriteAsJsonAsync(new { error = exceptionHandlerFeature.Error.Message });
            });
        });

        app.UseAuthorization();

        // Map other endpoints, including controllers
        app.MapControllers();

        app.Run();
    }
}