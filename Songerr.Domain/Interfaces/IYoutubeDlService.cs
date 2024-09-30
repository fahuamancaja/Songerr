using Songerr.Domain.Models;

namespace Songerr.Domain.Interfaces;

public interface IYoutubeDlService
{
    Task DownloadAudioFile(SongModel? songModel);
    Task GetSongMetadata(SongModel? songModel);
}