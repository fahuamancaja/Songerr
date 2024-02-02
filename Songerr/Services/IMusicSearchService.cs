using Songerr.Models;

namespace Songerr.Services
{
    public interface IMusicSearchService
    {
        Task<string> GetSongInfoAsync();
        Task<string> SearchSpotifyStructure(string fullPath);
    }
}
