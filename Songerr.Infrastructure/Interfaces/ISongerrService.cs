using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Infrastructure.Interfaces;

public interface ISongerrService
{
    Task<SongModel?> SongerrSongService(string videoTitle);
    Task<SongModel?> SongerrPlaylistService(SongModel songModel);
}