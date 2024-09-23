using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Infrastructure.Interfaces;

public interface IMusicSearchService
{
    //Task<string> GetSongInfoAsync();
    Task SearchSpotifyMetaData(SongModel? songModel);
}