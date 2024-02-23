using Songerr.Models;
using YoutubeExplode.Playlists;

namespace Songerr.Repository
{
    public interface IYoutubeDlRepository
    {
        Task DownloadVideoAsMp3(PlaylistModel videoId);
        Task PlayListDownloadVideoAsMp3(PlaylistModel playlistVideo);
        Task<List<PlaylistModel>> GetIPlaylistVideoFromPlaylist(string playlistId);
    }
}
