using Songerr.Models;
using YoutubeExplode.Playlists;

namespace Songerr.Services
{
    public interface ISongerrService
    {
        ValueTask<string> DownloadFirstVideoAsMp3(SongModel playListVideo);
        ValueTask<string> GetSingleMp3BasedOnUrl(string videoTitle);
    }

}
