using Songerr.Domain.Models;

namespace Songerr.Domain.Interfaces;

public interface IPlaylistService
{
    Task<List<SongModel>?> DownloadPlaylistAudioFiles(string? playlistId);
}