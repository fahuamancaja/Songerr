using AutoMapper;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.PayloadModels;
using YoutubeExplode.Videos;

namespace Songerr.Domain.Services;

public class YoutubeDlService(IMapper mapper, IYoutubeDlClient youtubeDlClient) : IYoutubeDlService
{
    public async Task DownloadAudioFile(SongModel? songModel)
    {
        songModel!.FilePath = await youtubeDlClient.DownloadAudioFile(songModel);
    }

    public async Task GetSongMetadataFromSongId(SongModel? songModel)
    {
        var result = await youtubeDlClient.GetSongMetadata(songModel);
        
        mapper.Map(result, songModel);

        // Assuming songModel is an instance of a class with a property Author
        string[] parts = songModel?.Author?.Split('-')!;
        songModel!.Author = parts[0].Trim();
    }
}