namespace Songerr.Services
{
    using Songerr.Models;
    using Songerr.Repository;
    using System.Threading.Tasks;
    using YoutubeExplode.Playlists;

    public class SongerrService : ISongerrService
    {
        private readonly SongerrSettings _songerrSettings;
        private readonly IYoutubeDlRepository _youtubeDlRepository;
        private readonly IYoutubeRepository _youtubeRepository;
        private readonly IParserService _parserService;
        private readonly IMusicSearchService _musicSearchService;

        public SongerrService(SongerrSettings songerrSettings, IYoutubeDlRepository youtubeDlRepository, IYoutubeRepository youtubeRepository, IParserService parserService, IMusicSearchService musicSearchService)
        {
            _songerrSettings = songerrSettings;
            _youtubeDlRepository = youtubeDlRepository;
            _youtubeRepository = youtubeRepository;
            _parserService = parserService;
            _musicSearchService = musicSearchService;
        }

        public async Task DownloadFirstVideoAsMp3(SongModel playListVideo)
        {
            try
            {
                await _youtubeDlRepository.DownloadVideoAsMp3(playListVideo);
                await _parserService.MoveFileToCorrectLocationAsync(playListVideo);
                await _musicSearchService.SearchSpotifyMetaData(playListVideo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public async Task<SongModel> GetSingleMp3BasedOnUrl(string videoId)
        {
            var songModel = new SongModel() { Id = videoId };
            try
            {
                _parserService.ParseVideoUrl(songModel);

                if (songModel.Id == null)
                {
                    throw new ArgumentNullException(nameof(songModel.Id), "Video ID or URL cannot be null.");
                }
                await _youtubeDlRepository.GetSongMetadataFromSongId(songModel);
                await _musicSearchService.SearchSpotifyMetaData(songModel);

                await _youtubeDlRepository.DownloadVideoAsMp3(songModel);
                await _parserService.MoveFileToCorrectLocationAsync(songModel);
                await _parserService.AddMetaDataToFile(songModel);

                return songModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
