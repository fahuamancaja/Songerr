using Microsoft.AspNetCore.Diagnostics;
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
        // Exception Handler
        services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();

        // Add services to the container.
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAutoMapper(typeof(Program));
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<Program>(); });

        // Register Services
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

        //Register Settings
        services.Configure<YoutubeSettings>(configuration.GetSection("AppSettings:Youtube"));
        services.Configure<SpotifySettings>(configuration.GetSection("AppSettings:Spotify"));
        services.Configure<SongerrSettings>(configuration.GetSection("Songerr.Application"));
        services.Configure<LocalSettings>(configuration.GetSection("AppSettings:LocalSettings"));

        return services;
    }
}