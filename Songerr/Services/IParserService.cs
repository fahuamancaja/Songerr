using Songerr.Models;

namespace Songerr.Services
{
    public interface IParserService
    {
        void ParseVideoUrl(SongModel playlistModel);
        Task MoveFileToCorrectLocationAsync(SongModel playListModel);
        Task AddMetaDataToFile(SongModel songModel);
    }
}
