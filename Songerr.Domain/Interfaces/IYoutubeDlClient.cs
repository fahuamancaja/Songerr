using Songerr.Domain.Models;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace Songerr.Domain.Interfaces;

public interface IYoutubeDlClient
{
    Task<IReadOnlyList<PlaylistVideo>?> GetPlaylistMetadata(string? playlistId);
    Task<string?> DownloadAudioFile(SongModel? songModel);
    Task<Video?> GetSongMetadata(SongModel? songModel);
}