using Songerr.Models;

namespace Songerr.Services
{
    public interface IMusicSearchService
    {
        Task<string> GetSongInfoAsync();
    }
}
