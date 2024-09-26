# Songerr API

Songerr API provides seamless integration for downloading audio files using YouTubeDLSharp, YoutubeExplode, and Spotify for metadata information. This API works in tandem with the [SongerrApp mobile phone app](https://github.com/fahuamancaja/SongerrApp/) to deliver a holistic music experience.

## Disclaimer

**For Educational Purposes Only**

This project is intended solely for educational purposes. The author and contributors do not condone or support the downloading of copyrighted material from YouTube or any other source without the appropriate rights or ownership. Users are responsible for ensuring they comply with all applicable laws and terms of service. Use this software responsibly and ethically.

## Prerequisites

Ensure you have the following installed on your machine:
- .NET Core 8.0 SDK
- FFmpeg
- yt-dlp (YouTube-DL successor)
- Docker

## Using Docker

The application utilizes Docker for containerization and supports Elasticsearch for enhanced logging and data management.

### Dockerfile

The `Dockerfile` defines the image building instructions for the Songerr API. The image can be built using the following command:

```bash
docker build -t songerr-api:latest .
```

### Docker Compose

The `docker-compose.yml` file orchestrates the deployment of the Songerr API along with Elasticsearch and Kibana for logging and data visualization. Here is a summary of the `docker-compose.yml` setup:

#### Services
- **Elasticsearch**: Provides full-text search, logging, and analytics capabilities.
- **Kibana**: Visualizes data stored in Elasticsearch.
- **Songerr API**: The main API service.

#### Usage

To start the application with Docker Compose, run the following command:

```bash
docker-compose up -d
```

This command will set up and start the Elasticsearch, Kibana, and Songerr API services.

### Docker Compose Configuration

Below is the `docker-compose.yml` configuration:

```yaml
version: '3.7'

services:
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:7.15.0
    container_name: elasticsearch
    environment:
      - node.name=elasticsearch
      - cluster.name=es-docker-cluster
      - discovery.type=single-node
      - bootstrap.memory_lock=true
      - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
    ulimits:
      memlock:
        soft: -1
        hard: -1
    volumes:
      - esdata:/usr/share/elasticsearch/data
    ports:
      - 9200:9200
    networks:
      - elk

  kibana:
    image: docker.elastic.co/kibana/kibana:7.15.0
    container_name: kibana
    ports:
      - 5601:5601
    environment:
      ELASTICSEARCH_URL: http://elasticsearch:9200
      ELASTICSEARCH_HOSTS: http://elasticsearch:9200
    networks:
      - elk
    depends_on:
      - elasticsearch

  songerr-api:
    image: songerr-api:latest
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5002:5002"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      - elasticsearch
    networks:
      - elk
    volumes:
      - E:\Music:/app/music

networks:
  elk:
    driver: bridge

volumes:
  esdata:
    driver: local
```

### Configuration Summary

- **Elasticsearch**:
  - Runs on port `9200`
  - Configured to use a single node for simplicity
- **Kibana**:
  - Runs on port `5601`
  - Connected to Elasticsearch
- **Songerr API**:
  - Exposes ports `5002`
  - Depends on Elasticsearch
  - Volume `E:\Music` mapped to `/app/music` inside the container

## Setting Up

### Obtaining API Keys

You need to obtain several API keys and secrets to configure the application.

1. **Spotify ClientId and ClientSecret**
   - Register your application at the [Spotify Developer Dashboard](https://developer.spotify.com/dashboard/applications).
   - Create a new project to get your ClientId and ClientSecret.

2. **YouTube ApiKey**
   - Go to the [Google Cloud Console](https://console.cloud.google.com/).
   - Create a new project or select an existing one.
   - Enable the YouTube Data API v3 for your project.
   - Create credentials for an API Key.

3. **MyApiKey**
   - Create a unique key that will be used to secure communication between your Songerr API and the SongerrApp mobile phone app.
   - This key should be set up in both the API and the mobile app.

### Configuration

Place the following configuration in your `appsettings.json` file:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware": "Information"
    },
    "Console": {
      "IncludeScopes": false,
      "TimestampFormat": "yyyy-MM-dd HH:mm:ss "
    },
    "File": {
      "Path": "Logs/log-.txt",
      "Append": "true",
      "IncludeScopes": false,
      "TimestampFormat": "yyyyMMddHHmmss"
    }
  },
  "AllowedHosts": "*",
  "AppSettings": {
    "Youtube": {
      "ApiKey": "YOUR_YOUTUBE_API_KEY",
      "AppName": "Songerr"
    },
    "Spotify": {
      "ClientId": "YOUR_SPOTIFY_CLIENT_ID",
      "ClientSecret": "YOUR_SPOTIFY_CLIENT_SECRET"
    },
    "LocalSettings": {
      "DownloadPath": "/app/music",
      "OperatingSystem": "Linux",
      "YoutubeDLPath": "/venv/bin/yt-dlp",
      "FFmpegPath": "/venv/bin/ffmpeg.exe"
    }
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://*:5002"
      }
    }
  },
  "ApiKeys": {
    "MyApiKey": "YOUR_UNIQUE_API_KEY"
  }
}
```

## Registering Services

In the `ServiceCollectionExtension.cs`, various services and settings are registered. Below is a brief list of the key services:

- **Exception Handler**: Handles global exceptions.
- **Controllers**: Adds support for MVC controllers.
- **Swagger**: Enables API documentation using Swagger.
- **AutoMapper and MediatR**: Registers AutoMapper and MediatR services.
- **Scoped Services**: Registers various scoped services such as `IPlaylistService`, `IYoutubeDlClient`, `IMusicSearchService`, `ISongerrService`, etc.
- **Singleton Services**: Registers singletons for `YoutubeDL` and `YoutubeClient`.
- **Commands**: Registers commands for song processing.
- **Settings**: Configures app settings for YouTube, Spotify, and local settings.

## Main Program Configuration

In `Program.cs`, the main configuration and middleware setup is defined. Here is a summary:

```csharp
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

        app.MapHealthChecksUI(setup => { setup.AddCustomStylesheet("wwwroot/Assets/dotnet.css"); });

        app.UseAuthorization();

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
```

## Viewing Logs in Kibana

To visualize and search through your logs in Kibana, you need to set up an index pattern. Follow these steps:

1. Open Kibana (e.g., by navigating to `http://localhost:5601` in your browser).
2. Go to the "Management" section in the sidebar.
3. Click on "Index Patterns".
4. Click on "Create index pattern".
5. Enter the pattern `aspnetcore-logs-*` and proceed with the setup by selecting the correct timestamp field.
6. Save the index pattern.

You should now be able to explore and analyze your logs using Kibana's interface.

## API Endpoints

The Songerr API includes two primary endpoints managed by `SongerrController`. Here are the details:

### SongerrController

The `SongerrController` handles requests related to song downloads and playlist processing.

#### Endpoints

1. **POST /api/songerr**
   - **Description**: Converts a video URL to an MP3 file.
   - **Request Body**:
     ```json
     {
       "url": "https://www.youtube.com/watch?v=example"
     }
     ```
   - **cURL Example**:
     ```bash
     curl -X POST http://localhost:5002/api/songerr \
     -H "Content-Type: application/json" \
     -d '{ "url": "https://www.youtube.com/watch?v=example" }'
     ```

2. **GET /api/songerr/DownloadPlaylistSongs**
   - **Description**: Converts all videos in a playlist to MP3 files.
   - **Request Parameters**: `playlistId` (string)
   - **cURL Example**:
     ```bash
     curl -X GET "http://localhost:5002/api/songerr/DownloadPlaylistSongs?playlistId=examplePlaylistId"
     ```

### Example Implementation

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Songerr.Application.Application.Command;
using Songerr.Domain.Models;

namespace Songerr.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongerrController : ControllerBase
{
    private readonly IMediator _mediator;

    public SongerrController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SongInput songInput)
    {
        Log.Information($"Received request to convert video URL: {songInput.Url}");

        var songModel = await _mediator.Send(new DownloadVideoAsMp3Command { Url = songInput.Url }).ConfigureAwait(false);
        Log.Information($"Successfully converted video URL. MP3 file path: {songModel.FilePath}");
        
        return Ok($"Completed {songModel.Title}");
    }

    [HttpGet("DownloadPlaylistSongs")]
    public async Task<IActionResult> DownloadPlaylistSongs(string playlistId)
    {
        Log.Information($"Received request to convert Playlist Id: {playlistId}");
        
        return Ok($"Completed:{(await _mediator.Send(new DownloadPlaylistSongsCommand { PlaylistId = playlistId })
            .ConfigureAwait(false)).Count}");
    }
}
```

## Usage

- **Download**: Use the API to download audio files from YouTube and other supported sources.
- **Metadata Extraction**: Extract metadata using Spotify and YoutubeExplode.
- **File Management**: Manage files and move them to correct locations based on metadata.

## Contributing

Feel free to fork the repository and submit pull requests. For major changes, please open an issue first to discuss what you would like to change.

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

For inquiries and support, please reach out via the contact information provided in the [Profile Page](https://github.com/fahuamancaja/).

Enjoy using Songerr API! 🎵
