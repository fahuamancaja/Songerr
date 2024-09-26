using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Songerr.Domain.Factories;
using Songerr.Domain.Services;
using Songerr.Infrastructure.ApiClients;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.OptionSettings;
using YoutubeDLSharp;
using YoutubeExplode;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Songerr.Application.Middleware;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Existing service registrations

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAutoMapper(typeof(Program));
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<Program>(); });

        // Register authorization services
        services.AddAuthorization();
        services.AddAuthentication(/* scheme configurations if needed */);

        // Register other services
        services.AddScoped<IPlaylistService, PlaylistService>();
        services.AddScoped<IYoutubeDlClient, YoutubeDlClient>();
        services.AddScoped<IMusicSearchService, MusicSearchService>();
        services.AddScoped<ISongerrService, SongerrService>();
        services.AddScoped<IParserService, ParserService>();
        services.AddScoped<ISpotifyClientSearch, SpotifyClient>();
        services.AddScoped<IYoutubeDlService, YoutubeDlService>();
        services.AddSingleton<YoutubeDL>();
        services.AddSingleton<YoutubeClient>();

        // Register commands
        services.AddTransient<ISongProcessingCommand, ParseVideoUrlCommand>();
        services.AddTransient<ISongProcessingCommand, GetSongMetadataCommand>();
        services.AddTransient<ISongProcessingCommand, SearchSpotifyMetadataCommand>();
        services.AddTransient<ISongProcessingCommand, DownloadAudioFileCommand>();
        services.AddTransient<ISongProcessingCommand, MoveFileToCorrectLocationCommand>();
        services.AddTransient<ISongProcessingCommand, AddMetadataToFileCommand>();

        // Register Settings
        services.Configure<YoutubeSettings>(configuration.GetSection("AppSettings:Youtube"));
        services.Configure<SpotifySettings>(configuration.GetSection("AppSettings:Spotify"));
        services.Configure<SongerrSettings>(configuration.GetSection("Songerr.Application"));
        services.Configure<LocalSettings>(configuration.GetSection("AppSettings:LocalSettings"));

        // Add basic health check services
        services.AddHealthChecks()
                .AddCheck("API Health Check", () =>
                {
                    var isHealthy = true; // Simulate a health check
                    return isHealthy ? HealthCheckResult.Healthy("The check indicates a healthy state.") :
                                       HealthCheckResult.Unhealthy("The check indicates an unhealthy state.");
                }, tags: new[] { "AspNetCore" }, new TimeSpan(0, 1, 0));

        // Add HealthChecks UI
        var healthCheckUiSection = configuration.GetSection("HealthCheckUI:HealthChecks");
        foreach (var healthCheck in healthCheckUiSection.GetChildren())
        {
            var name = healthCheck.GetValue<string>("Name");
            var uri = healthCheck.GetValue<string>("Uri");
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(uri))
            {
                services.AddHealthChecksUI(opt => opt.AddHealthCheckEndpoint(name, uri)).AddInMemoryStorage();
            }
        }


        // Remove IExceptionHandler registration
        return services;
    }
}