using Songerr.Domain.Models;

namespace Songerr.Domain.Interfaces;

public interface ISpotifyClientSearch
{
    Task<string?> GetSpotifyAccessTokenAsync();
    Task<SpotifyResults?> GetSpotifyMetaData(SongModel? songModel, string accessToken);
}