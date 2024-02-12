using Songerr.Services;
using Microsoft.Extensions.Configuration;
using Songerr.Models;
using Songerr;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Build Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console() // Ensure console logging is enabled
    .CreateLogger();

// Add Serilog to the logging pipeline
builder.Host.UseSerilog();

// Get settings from Configuration Youtube
string apiKey = builder.Configuration["AppSettings:Youtube:ApiKey"];
string appName = builder.Configuration["AppSettings:Youtube:AppName"];

// Get settings from Configuration Spotify
string clientId = builder.Configuration["AppSettings:Spotify:client_id"];
string clientSecret = builder.Configuration["AppSettings:Spotify:client_secret"];

//Get Songerr Settings
var songerrSettings = builder.Configuration.GetSection("Songerr").Get<SongerrSettings>();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Add services to the container.
builder.Services.AddSingleton<ISpotifyService, SpotifyPlaylistService>(provider =>
    new SpotifyPlaylistService(clientId, clientSecret));
builder.Services.AddSingleton<IMusicSearchService, MusicSearchService>(provider =>
    new MusicSearchService(clientId,clientSecret));
builder.Services.AddSingleton<ISongerrService, SongerrService>(provider =>
    new SongerrService(apiKey, appName, songerrSettings));
builder.Services.AddSingleton<IPlaylistRetriever, YoutubPlaylistService>(provider =>
    new YoutubPlaylistService(apiKey));


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
