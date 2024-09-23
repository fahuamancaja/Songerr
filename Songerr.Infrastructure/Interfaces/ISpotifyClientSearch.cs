using Songerr.Infrastructure.Models;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Infrastructure.Interfaces;

public interface ISpotifyClientSearch
{
    Task<string> GetSpotifyAccessTokenAsync();
    Task<SpotifyResults?> GetSpotifyMetaData(SongModel? songModel, string accessToken);
}
