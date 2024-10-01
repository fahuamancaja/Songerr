using Serilog;
using Songerr.Domain.Factories;
using Songerr.Domain.Interfaces;
using Songerr.Domain.Models;

namespace Songerr.Domain.Services;

public class SongerrService(
    IYoutubeDlService youtubeDlService,
    IParserService parserService,
    IMusicSearchService musicSearchService)
    : ISongerrService
{
    private readonly ISongProcessingCommand[] _commands =
    [
        new ParseVideoUrlCommand(parserService),
        new GetSongMetadataCommand(youtubeDlService),
        new SearchSpotifyMetadataCommand(musicSearchService),
        new DownloadAudioFileCommand(youtubeDlService),
        new MoveFileToCorrectLocationCommand(parserService),
        new AddMetadataToFileCommand(parserService)
    ];

    private readonly ISongProcessingCommand[] _playlistCommands =
    [
        new SearchSpotifyMetadataCommand(musicSearchService),
        new DownloadAudioFileCommand(youtubeDlService),
        new MoveFileToCorrectLocationCommand(parserService),
        new AddMetadataToFileCommand(parserService)
    ];

    private async Task ExecuteCommandsAsync(IEnumerable<ISongProcessingCommand> commands, SongModel songModel)
    {
        foreach (var command in commands)
        {
            await command.ExecuteAsync(songModel).ConfigureAwait(false);

            if (command is ParseVideoUrlCommand && songModel.Id == null)
                throw new ArgumentNullException(nameof(songModel.Id), "Song ID cannot be null.");

            Log.Information($"Executed command: {command.GetType().Name}");
        }
    }

    public async Task<SongModel?> SongerrSongService(string videoId)
    {
        var songModel = new SongModel { Id = videoId };

        Log.Information($"Starting to process video ID: {videoId}");

        await ExecuteCommandsAsync(_commands, songModel).ConfigureAwait(false);

        Log.Information($"Successfully processed video ID: {videoId}");
        return songModel;
    }

    public async Task<SongModel?> SongerrPlaylistService(SongModel songModel)
    {
        Log.Information($"Starting to process video ID: {songModel.Id}");

        await ExecuteCommandsAsync(_playlistCommands, songModel).ConfigureAwait(false);

        Log.Information($"Successfully processed video ID: {songModel.Id}");
        return songModel;
    }
}