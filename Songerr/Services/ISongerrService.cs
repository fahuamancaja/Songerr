using Songerr.Models;
using YoutubeExplode.Playlists;

namespace Songerr.Services
{
    public interface ISongerrService
    {
        Task DownloadFirstVideoAsMp3(SongModel playListVideo);
        Task<SongModel> GetSingleMp3BasedOnUrl(string videoTitle);
    }

}
