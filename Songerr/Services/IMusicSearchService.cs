using Songerr.Models;

namespace Songerr.Services
{
    public interface IMusicSearchService
    {
        Task<SpotifyResults> GetSongInfoAsync(string songName);
    }
}
