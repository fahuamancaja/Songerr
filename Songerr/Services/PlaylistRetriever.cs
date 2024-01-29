using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Songerr.Services
{
    public class PlaylistRetriever : IPlaylistRetriever
    {
        private readonly YouTubeService _youtubeService;

        public PlaylistRetriever(string apiKey)
        {
            _youtubeService = new YouTubeService(new BaseClientService.Initializer() { 
             ApiKey = apiKey });
        }

        public async Task<IEnumerable<string>> GetPlaylistTitlesAsync(string playlistId)
        {
            var nextPageToken = "";
            var titles = new List<string>();

            do
            {
                var request = _youtubeService.PlaylistItems.List("snippet");
                request.PlaylistId = playlistId;
                request.MaxResults = 50;
                request.PageToken = nextPageToken;

                var response = await request.ExecuteAsync();

                foreach (var item in response.Items)
                {
                    titles.Add($"{item.Snippet.Title}");
                }

                nextPageToken = response.NextPageToken;
            }
            while (!string.IsNullOrEmpty(nextPageToken));

            return titles;
        }
    }
}
