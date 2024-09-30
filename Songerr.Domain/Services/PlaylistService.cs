using AutoMapper;
using Serilog;
using Songerr.Domain.Interfaces;
using Songerr.Domain.Models;

namespace Songerr.Domain.Services;

public class PlaylistService(IMapper mapper, IYoutubeDlClient youtubeDlClient, ISongerrService songerrService)
    : IPlaylistService
{
    public async Task<List<SongModel>?> DownloadPlaylistAudioFiles(string? playlistId)
    {
        var playListModels = await youtubeDlClient.GetPlaylistMetadata(playlistId);

        if (playListModels == null) return null;
        Log.Information($"Obtained playlist models from Youtube Dl for {playListModels.Count} audio files");

        var songModels = playListModels
            .Select(mapper.Map<SongModel>)
            .Where(mappedResult => mappedResult != null) // Filter out nulls after mapping
            .ToList();

        if (songModels.Count == 0) return [];

        foreach (var songModel in songModels)
            await songerrService.SongerrPlaylistService(songModel).ConfigureAwait(false);

        return songModels;
    }
}