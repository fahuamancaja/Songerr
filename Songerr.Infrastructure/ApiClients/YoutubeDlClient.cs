using Microsoft.Extensions.Options;
using Serilog;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.OptionSettings;
using Songerr.Infrastructure.PayloadModels;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace Songerr.Infrastructure.ApiClients;

public class YoutubeDlClient(IOptions<LocalSettings> settings, YoutubeDL youtubeDl, YoutubeClient youtubeClient)
    : IYoutubeDlClient
{
    private readonly LocalSettings _settings = settings.Value;

    
    public async Task<IReadOnlyList<PlaylistVideo>?> GetPlaylistMetadata(string? playlistId)
    {
        var playlistUrl = $"https://music.youtube.com/playlist?list={playlistId}";
        return await youtubeClient.Playlists.GetVideosAsync(playlistUrl);
    }
    
    public async Task<string?> DownloadAudioFile(SongModel? songModel)
    {
        try
        {
            var url = $"https://music.youtube.com/watch?v={songModel?.Id}";
            Log.Information(_settings.OperatingSystem!);

            var ytdl = _settings.OperatingSystem == "Linux" ? 
                new YoutubeDL { YoutubeDLPath = _settings.YoutubeDLPath, FFmpegPath = _settings.FFmpegPath } : 
                youtubeDl;

            var result = await ytdl.RunAudioDownload(url, AudioConversionFormat.Opus).ConfigureAwait(false);
            var res = result.Data;
        
            Log.Information($"Downloaded opus audio file {res}");
            return res;
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            Log.Error($"Error downloading audio file: {ex.Message}");
            return null;
        }
    }

    public async Task<Video?> GetSongMetadata(SongModel? songModel)
    {
        try
        {
            var playlistUrl = $"https://music.youtube.com/watch?v={songModel?.Id}";
            return await youtubeClient.Videos.GetAsync(playlistUrl).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            Log.Error($"Error fetching song metadata: {ex.Message}");
            return null;
        }
    }
}