using Songerr.Infrastructure.OptionSettings;

namespace Songerr.Tests.UnitTests.InfrastructureTests;

public class YoutubeSettingsTests
{
    [Fact]
    public void YoutubeSettings_GettersAndSetters_WorkCorrectly()
    {
        // Arrange
        var settings = new YoutubeSettings();
        var apiName = "YouTubeAPI";
        var apiKey = "YOUR_API_KEY";

        // Act
        settings.ApiName = apiName;
        settings.ApiKey = apiKey;

        // Assert
        Assert.Equal(apiName, settings.ApiName);
        Assert.Equal(apiKey, settings.ApiKey);
    }
}

public class SpotifySettingsTests
{
    [Fact]
    public void SpotifySettings_GettersAndSetters_WorkCorrectly()
    {
        // Arrange
        var settings = new SpotifySettings();
        var clientId = "YOUR_CLIENT_ID";
        var clientSecret = "YOUR_CLIENT_SECRET";

        // Act
        settings.ClientId = clientId;
        settings.ClientSecret = clientSecret;

        // Assert
        Assert.Equal(clientId, settings.ClientId);
        Assert.Equal(clientSecret, settings.ClientSecret);
    }
}

public class SongerrSettingsTests
{
    [Fact]
    public void SongerrSettings_GettersAndSetters_WorkCorrectly()
    {
        // Arrange
        var settings = new SongerrSettings();
        var applicationName = "SongerrApp";

        // Act
        settings.ApplicationName = applicationName;

        // Assert
        Assert.Equal(applicationName, settings.ApplicationName);
    }
}

public class LocalSettingsTests
{
    [Fact]
    public void LocalSettings_GettersAndSetters_WorkCorrectly()
    {
        // Arrange
        var settings = new LocalSettings();
        var downloadPath = "C:\\Downloads";
        var youtubeDLPath = "C:\\YouTubeDL\\youtube-dl.exe";
        var ffmpegPath = "C:\\FFmpeg\\ffmpeg.exe";
        var operatingSystem = "Windows";

        // Act
        settings.DownloadPath = downloadPath;
        settings.YoutubeDLPath = youtubeDLPath;
        settings.FFmpegPath = ffmpegPath;
        settings.OperatingSystem = operatingSystem;

        // Assert
        Assert.Equal(downloadPath, settings.DownloadPath);
        Assert.Equal(youtubeDLPath, settings.YoutubeDLPath);
        Assert.Equal(ffmpegPath, settings.FFmpegPath);
        Assert.Equal(operatingSystem, settings.OperatingSystem);
    }
}