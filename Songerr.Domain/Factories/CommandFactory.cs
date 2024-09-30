using Songerr.Domain.Interfaces;
using Songerr.Domain.Models;

namespace Songerr.Domain.Factories;

public interface ISongProcessingCommand
{
    Task ExecuteAsync(SongModel songModel);
}

public class ParseVideoUrlCommand(IParserService parserService) : ISongProcessingCommand
{
    public Task ExecuteAsync(SongModel songModel)
    {
        parserService.ParseVideoUrl(songModel);
        return Task.CompletedTask;
    }
}

public class GetSongMetadataCommand(IYoutubeDlService youtubeDlService) : ISongProcessingCommand
{
    public Task ExecuteAsync(SongModel songModel)
    {
        return youtubeDlService.GetSongMetadata(songModel);
    }
}

public class SearchSpotifyMetadataCommand(IMusicSearchService searchService) : ISongProcessingCommand
{
    public Task ExecuteAsync(SongModel songModel)
    {
        return searchService.SearchSpotifyMetadata(songModel);
    }
}

public class DownloadAudioFileCommand(IYoutubeDlService youtubeDlService) : ISongProcessingCommand
{
    public Task ExecuteAsync(SongModel songModel)
    {
        return youtubeDlService.DownloadAudioFile(songModel);
    }
}

public class MoveFileToCorrectLocationCommand(IParserService parserService) : ISongProcessingCommand
{
    public Task ExecuteAsync(SongModel songModel)
    {
        return parserService.MoveFileToCorrectLocation(songModel);
    }
}

public class AddMetadataToFileCommand(IParserService parserService) : ISongProcessingCommand
{
    public Task ExecuteAsync(SongModel songModel)
    {
        return parserService.AddMetaDataToFile(songModel);
    }
}