namespace Songerr.Domain.Models.OptionSettings;

public class YoutubeSettings
{
    public string? ApiName { get; set; }
    public string? ApiKey { get; set; }
}

public class SpotifySettings
{
    public string? ClientSecret { get; set; }
    public string? ClientId { get; set; }
}

public class SongerrSettings
{
    public string? ApplicationName { get; set; }
}

public class LocalSettings
{
    public string? DownloadPath { get; set; }
    public string? YoutubeDLPath { get; set; }
    public string? FFmpegPath { get; set; }
    public string? OperatingSystem { get; set; }
}