using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Infrastructure.Interfaces;

public interface IYoutubeDlService
{
    Task DownloadAudioFile(SongModel? songModel);
    Task GetSongMetadata(SongModel? songModel);
}