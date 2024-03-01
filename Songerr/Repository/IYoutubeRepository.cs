using Songerr.Models;
using YoutubeExplode.Playlists;

namespace Songerr.Repository
{
    public interface IYoutubeRepository
    {
        Task<List<string>> YoutubeSearchListGetIds(string searchString, long maxResults);
        Task<string> YoutubeSortByStatsReturnFirst(List<string> videoIdsList);
    }
}
