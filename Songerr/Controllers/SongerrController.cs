using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using Songerr.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Songerr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongerrController : ControllerBase
    {
        private readonly ISongerrService _songerrService;
        private readonly IMusicSearchService _musicSearchService;
        private readonly IPlaylistRetriever _playlistRetrieverService;
        private readonly ISpotifyService _spotifyService;

        public SongerrController(ISongerrService songerrService, IMusicSearchService musicSearchService, IPlaylistRetriever playlistRetrieverService, ISpotifyService spotifyService)
        {
            _songerrService = songerrService;
            _musicSearchService = musicSearchService;
            _playlistRetrieverService = playlistRetrieverService;
            _spotifyService = spotifyService;
        }

        public class SongInput
        {
            public List<string> Titles { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SongInput input)
        {
            var mp3PathResults = new List<string>();
            var searchResults = new List<string>();
            foreach (var title in input.Titles)
            {
                var mp3Path = await _songerrService.DownloadFirstVideoAsMp3(title);
                var searchResult = await _musicSearchService.SearchSpotifyStructure(mp3Path);

                mp3PathResults.Add(mp3Path);
                searchResults.Add(searchResult);
            }

            var calculatedComplete = CalculateCompletionRate(searchResults);
            return Ok(calculatedComplete);
        }

        [HttpGet("GetSongInfo")]
        public async Task<IActionResult> GetSongInfo()
        {
            var songInfo = await _musicSearchService.GetSongInfoAsync();

            if (songInfo == null)
            {
                return NotFound($"No song found with the name.");
            }

            return Ok(songInfo);
        }
        [HttpGet("GetPlaylistTitles")]
        public async Task<IActionResult> GetPlaylistTitles(string playlistId)
        {

            var request = await _playlistRetrieverService.GetPlaylistTitlesAsync(playlistId);

            return Ok(request);
        }

        [HttpGet("GetSpotifyPlaylistTitles")]
        public async Task<IActionResult> GetSpotifyPlaylistTitles(string playlistId)
        {
            var request = await _spotifyService.GetSongTitlesAndArtistsAsync(playlistId);
            return Ok(request);
        }
        private static string CalculateCompletionRate(List<string> list)
        {
            int totalCount = list.Count;
            int completeCount = list.Count(x => x.ToLower() == "complete"); // Assuming a non-empty string is considered complete
            double completionRate = (double)completeCount / totalCount;
            return $"{completeCount}/{totalCount} completed";
        }

        private void CheckAndCreateLogFile(string mp3Path, string searchResult)
        {
            string logFilePath = "mp3.log";
            if (!System.IO.File.Exists(logFilePath))
            {
                using (StreamWriter sw = System.IO.File.CreateText(logFilePath))
                {
                    sw.WriteLine($"MP3 Path: {mp3Path}");
                    sw.WriteLine($"Search Result: {searchResult}");
                }
            }
            else
            {
                using (StreamWriter sw = System.IO.File.AppendText(logFilePath))
                {
                    sw.WriteLine($"MP3 Path: {mp3Path}");
                    sw.WriteLine($"Search Result: {searchResult}");
                }
            }
        }

    }

}
}
