using AutoMapper;
using Serilog;
using Songerr;
using Songerr.Models;
using Songerr.Repository;
using Songerr.Services;
using System.Reflection;
using YoutubeDLSharp;
using YoutubeExplode;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Configuration Settings
var youtubeSettings = builder.Configuration.GetSection("AppSettings:Youtube").Get<YoutubeSettings>();
var spotifySettings = builder.Configuration.GetSection("AppSettings:Spotify").Get<SpotifySettings>();
var songerrSettings = builder.Configuration.GetSection("Songerr").Get<SongerrSettings>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Register services
builder.Services.AddSingleton<IYoutubeDlRepository, YoutubeDlRepository>();
builder.Services.AddSingleton<ISpotifyService, SpotifyService>(provider =>
    new SpotifyService(spotifySettings.ClientId, spotifySettings.ClientSecret));
builder.Services.AddSingleton<IMusicSearchService, MusicSearchService>(provider =>
    new MusicSearchService(spotifySettings.ClientId, spotifySettings.ClientSecret));
builder.Services.AddSingleton<ISongerrService, SongerrService>(provider =>
    new SongerrService(songerrSettings,
                       new YoutubeDlRepository(provider.GetRequiredService<IMapper>(), new YoutubeDL(), new YoutubeClient()),
                       new YoutubeRepository(youtubeSettings.ApiKey, youtubeSettings.AppName, provider.GetRequiredService<IMapper>()),
                       new ParserService(),
                       new MusicSearchService(spotifySettings.ClientId, spotifySettings.ClientSecret)));
builder.Services.AddSingleton<IYoutubeRepository, YoutubeRepository>(provider =>
    new YoutubeRepository(youtubeSettings.ApiKey, youtubeSettings.AppName, provider.GetRequiredService<IMapper>()));
builder.Services.AddSingleton<IPlaylistRetriever, YoutubPlaylistService>(provider =>
    new YoutubPlaylistService(youtubeSettings.ApiKey,
                               new YoutubeRepository(youtubeSettings.ApiKey, youtubeSettings.AppName, provider.GetRequiredService<IMapper>()),
                               new YoutubeDlRepository(provider.GetRequiredService<IMapper>(), new YoutubeDL(), new YoutubeClient()),
                               new ParserService(),
                               provider.GetRequiredService<ISongerrService>(),
                               new MusicSearchService(spotifySettings.ClientId, spotifySettings.ClientSecret)));
builder.Services.AddSingleton<IParserService, ParserService>();

// Register YoutubeDL with the DI container
builder.Services.AddSingleton<YoutubeDL>();

// Register YoutubecLIENT with the DI container
builder.Services.AddSingleton<YoutubeClient>();

// Continue with other service registrations...
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ApiKeyMiddleware>();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Define configuration models for Youtube and Spotify
public class YoutubeSettings
{
    public string ApiKey { get; set; }
    public string AppName { get; set; }
}

public class SpotifySettings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
