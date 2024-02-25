namespace Songerr.Services
{
    using Songerr.Models;
    using Songerr.Repository;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using YoutubeExplode.Playlists;

    public class SongerrService : ISongerrService
    {
        private readonly SongerrSettings _songerrSettings;
        private readonly IYoutubeDlRepository _youtubeDlRepository;
        private readonly IYoutubeRepository _youtubeRepository;
        private readonly IParserService _parserService;

        public SongerrService(SongerrSettings songerrSettings, IYoutubeDlRepository youtubeDlRepository, IYoutubeRepository youtubeRepository, IParserService parserService)
        {
            _songerrSettings = songerrSettings;
            _youtubeDlRepository = youtubeDlRepository;
            _youtubeRepository = youtubeRepository;
            _parserService = parserService;
        }

        public async ValueTask<string> DownloadFirstVideoAsMp3(SongModel playListVideo)
        {
            try
            {
                //var firstVideoId = await GetFirstVideoId(videoTitle);
                //var videoInfo = await _youtubeRepository.GetVideoInfo(firstVideoId);
                await _youtubeDlRepository.PlayListDownloadVideoAsMp3(playListVideo);
                return _parserService.MoveFileToCorrectLocation(playListVideo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetFirstVideoId(string videoTitle)
        {
            var videoIds = await _youtubeRepository.YoutubeSearchListGetIds(videoTitle, _songerrSettings.MaxResults);
            return await _youtubeRepository.YoutubeSortByStatsReturnFirst(videoIds);
        }

        public async ValueTask<string> GetSingleMp3BasedOnUrl(string videoId)
        {
            var songModel = new SongModel() { Id = videoId};
            try
            {
                _parserService.ParseVideoUrl(songModel);

                if (songModel.Id == null)
                {
                    throw new ArgumentNullException(nameof(songModel.Id), "Video ID or URL cannot be null.");
                }
                await _youtubeDlRepository.GetSongMetadataFromSongId(songModel);

                await _youtubeDlRepository.DownloadVideoAsMp3(songModel);
                return _parserService.MoveFileToCorrectLocation(songModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
