using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode;


namespace Songerr.Services
{
    public class YoutubPlaylistService : IPlaylistRetriever
    {
        private readonly YouTubeService _youtubeService;

        public YoutubPlaylistService(string apiKey)
        {
            _youtubeService = new YouTubeService(new BaseClientService.Initializer() { 
             ApiKey = apiKey });
        }

        public async Task<List<string>> GetPlaylistTitlesAsync(string playlistId)
        {
            var nextPageToken = "";
            var titles = new List<string>();

            var youtube = new YoutubeClient();
            var playlistUrl = $"https://music.youtube.com/playlist?list={playlistId}";

            await foreach (var video in youtube.Playlists.GetVideosAsync(playlistUrl))
            {
                var title = video.Title;
                var author = video.Author;
                var spTitles = "\"" + RemoveSpecialCharacters($"{author} - {title},") + "\"" + ",";
                titles.Add(spTitles);
            }

            return titles;
        }
        private static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9 ]", "");
        }

    }
}
