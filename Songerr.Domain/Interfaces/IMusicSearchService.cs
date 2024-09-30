using Songerr.Domain.Models;

namespace Songerr.Domain.Interfaces;

public interface IMusicSearchService
{
    //Task<string> GetSongInfoAsync();
    Task SearchSpotifyMetadata(SongModel? songModel);
}