using Songerr.Models;

namespace Songerr.Services
{
    public interface IPlaylistRetriever
    {
        Task<List<SongModel>> DownloadPlaylistSongs(string playlistId);
    }
}
