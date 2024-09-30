using Songerr.Domain.Models;

namespace Songerr.Domain.Interfaces;

public interface ISongerrService
{
    Task<SongModel?> SongerrSongService(string videoTitle);
    Task<SongModel?> SongerrPlaylistService(SongModel songModel);
}