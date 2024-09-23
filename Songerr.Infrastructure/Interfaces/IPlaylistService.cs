using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Infrastructure.Interfaces;

public interface IPlaylistService
{
    Task<List<SongModel>?> DownloadPlaylistAudioFiles(string playlistId);
}