using Songerr.Models;

namespace Songerr.Services
{
    public interface IPlaylistRetriever
    {
        Task<List<SongModel>> DownloadPlaylistSongs(string playlistId);
        Task<List<SongModel>> DownloadPlaylistSongsRevised(string playlistId);
    }
}
