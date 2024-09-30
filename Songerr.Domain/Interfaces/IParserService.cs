using Songerr.Domain.Models;

namespace Songerr.Domain.Interfaces;

public interface IParserService
{
    void ParseVideoUrl(SongModel? songmodel);
    Task MoveFileToCorrectLocation(SongModel songModel);
    Task AddMetaDataToFile(SongModel? songModel);
}