using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json.Linq;
using Songerr.Models;
using YoutubeExplode.Playlists;
using YoutubeExplode;
using AutoMapper;

namespace Songerr.Repository
{
    public class YoutubeRepository : IYoutubeRepository
    {
        private readonly YouTubeService _youtubeService;
        private readonly IMapper _mapper;

        public YoutubeRepository(string apiKey, string appName, IMapper mapper)
        {
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = appName
            });
            _mapper = mapper;
        }

        public async Task<List<string>> YoutubeSearchListGetIds(string searchString, long maxResults)
        {
            var searchListRequest = _youtubeService.Search.List("snippet");
            searchListRequest.Q = searchString;
            searchListRequest.Type = "video";
            searchListRequest.MaxResults = maxResults;

            var searchListResponse = await searchListRequest.ExecuteAsync();
            return searchListResponse.Items.Select(item => item.Id.VideoId).ToList();
        }

        public async Task<string> YoutubeSortByStatsReturnFirst(List<string> videoIdsList)
        {
            var videoDetailsRequest = _youtubeService.Videos.List("statistics");
            videoDetailsRequest.Id = string.Join(",", videoIdsList);

            var videoDetailsResponse = await videoDetailsRequest.ExecuteAsync();
            var sortedVideos = videoDetailsResponse.Items.OrderByDescending(item => item.Statistics.ViewCount ?? 0).ToList();
            return sortedVideos[0].Id;
        }
        public async Task<(string Title, string Artist)> GetVideoInfo(string videoId)
        {
            var apiUrl = $"https://www.googleapis.com/youtube/v3/videos?part=snippet&id={videoId}&key={_youtubeService.ApiKey}";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                JObject parsed = JObject.Parse(jsonResponse);
                string title = parsed["items"][0]["snippet"]["title"].ToString();
                string artist = parsed["items"][0]["snippet"]["channelTitle"].ToString();

                artist = new string(artist.Where(c => !Path.GetInvalidPathChars().Contains(c)).ToArray());
                artist = artist.Replace(' ', '_').Replace('.', '_').Replace('-', '_');

                return (title, artist);
            }
        }
        
    }
}
