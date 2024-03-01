using Songerr.Models;

namespace Songerr.Services
{
    public interface IMusicSearchService
    {
        //Task<string> GetSongInfoAsync();
        Task SearchSpotifyMetaData(SongModel songModel);
    }
}
