using Songerr.Infrastructure.PayloadModels;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace Songerr.Infrastructure.Interfaces;

public interface IYoutubeDlClient
{
    Task<IReadOnlyList<PlaylistVideo>?> GetPlaylistMetadata(string? playlistId);
    Task<string?> DownloadAudioFile(SongModel? songModel);
    Task<Video?> GetSongMetadata(SongModel? songModel);
}