using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Songerr.Repository;
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
        private readonly IYoutubeRepository _youtubeRepository;
        private readonly IYoutubeDlRepository _youtubeDlRepository;
        private readonly IParserService _parserService;
        private readonly ISongerrService _songerrService;
        private readonly IMusicSearchService _musicSearchService;

        public YoutubPlaylistService(string apiKey, IYoutubeRepository youtubeRepository,IYoutubeDlRepository youtubeDlRepository, IParserService parserService, ISongerrService songerrService, IMusicSearchService musicSearchService)
        {
            _youtubeService = new YouTubeService(new BaseClientService.Initializer() { 
             ApiKey = apiKey });
            _youtubeRepository = youtubeRepository;
            _youtubeDlRepository = youtubeDlRepository;
            _parserService = parserService;
            _songerrService = songerrService;
            _musicSearchService = musicSearchService;
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
                var spTitles = $"{author} - {title}";
                spTitles = _parserService.RemoveSpecialCharacters(spTitles);
                titles.Add(spTitles);
            }

            return titles;
        }
        public async Task<List<string>> DownloadPlaylistSongs(string playlistId)
        {
            var playlistVideo = await _youtubeDlRepository.GetSongsMetadataFromPlaylistId(playlistId);

            var filePaths = new List<string>();
            foreach (var video in playlistVideo)
            {
                var filePath = await _songerrService.DownloadFirstVideoAsMp3(video);
                filePaths.Add(filePath);
            }

            var response = await GetSongDataSpotify(filePaths);
            return response;
        }
        private async Task<List<string>> GetSongDataSpotify(List<string> filePaths)
        {
            var results = new List<string>();
            foreach (var filePath in filePaths)
            {
                var response = await _musicSearchService.SearchSpotifyStructure(filePath);
                results.Add(response);
            }
            return results;
        }
    }
}
