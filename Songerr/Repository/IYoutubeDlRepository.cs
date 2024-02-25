using Songerr.Models;
using YoutubeExplode.Playlists;

namespace Songerr.Repository
{
    public interface IYoutubeDlRepository
    {
        Task DownloadVideoAsMp3(SongModel videoId);
        Task PlayListDownloadVideoAsMp3(SongModel playlistVideo);
        Task GetSongMetadataFromSongId(SongModel playlistModel);
        Task<List<SongModel>> GetSongsMetadataFromPlaylistId(string playlistId);
    }
}
