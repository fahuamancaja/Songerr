using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Songerr.Models;
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
        public async Task<List<SongModel>> DownloadPlaylistSongs(string playlistId)
        {
            var playlistVideos = await _youtubeDlRepository.GetSongsMetadataFromPlaylistId(playlistId);
            await DownloadMp3(playlistVideos);
            await GetSongDataSpotify(playlistVideos);
            return playlistVideos;
        }

        private async Task DownloadMp3(List<SongModel> playlistVideos)
        {
            foreach (var video in playlistVideos)
            {
                await _songerrService.DownloadFirstVideoAsMp3(video);
            }
        }

        private async Task GetSongDataSpotify(List<SongModel> songModels)
        {
            foreach (var songModel in songModels)
            {
                await _musicSearchService.SearchSpotifyMetaData(songModel);
            }
        }
    }
}
