using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Infrastructure.Interfaces;

public interface IParserService
{
    void ParseVideoUrl(SongModel? songmodel);
    Task MoveFileToCorrectLocation(SongModel songModel);
    Task AddMetaDataToFile(SongModel? songModel);
}