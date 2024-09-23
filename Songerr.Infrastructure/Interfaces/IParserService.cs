using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Infrastructure.Interfaces;

public interface IParserService
{
    void ParseVideoUrl(SongModel? songmodel);
    Task MoveFileToCorrectLocationAsync<T>(T playListModel) where T : IFileModel;
    Task AddMetaDataToFile(SongModel? songModel);
}