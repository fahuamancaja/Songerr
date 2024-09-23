using AutoMapper;
using Serilog;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Domain.Services;

public class PlaylistService(IMapper mapper, IYoutubeDlClient youtubeDlClient, ISongerrService songerrService) : IPlaylistService
{
    public async Task<List<SongModel>?> DownloadPlaylistAudioFiles(string? playlistId)
    {
        var playListModels = await youtubeDlClient.GetPlaylistMetadata(playlistId);

        if (playListModels == null) return null;
        Log.Information($"Obtained playlist models from Youtube Dl for {playListModels.Count} audio files");

         var songModels = playListModels.Any() ? playListModels.Select(mapper.Map<SongModel>).ToList() : null;

         if (songModels == null) return [];

         foreach (var songModel in songModels)
         {
             await songerrService.SongerrPlaylistService(songModel);
         }

         return songModels;
    }
}