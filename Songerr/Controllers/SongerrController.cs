using Google.Apis.YouTube.v3;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Songerr.Application.Command;
using Songerr.Application.Query;
using Songerr.Models;
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
        private readonly IMediator _mediator;

        public SongerrController(ISongerrService songerrService, IMusicSearchService musicSearchService, IPlaylistRetriever playlistRetrieverService, ISpotifyService spotifyService, IMediator mediator)
        {
            _songerrService = songerrService;
            _musicSearchService = musicSearchService;
            _playlistRetrieverService = playlistRetrieverService;
            _spotifyService = spotifyService;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SongInput input)
        {
            var mp3PathResults = new List<string>();
            var searchResults = new List<string>();
            foreach (var title in input.Titles)
            {
                var mp3Path = await _mediator.Send(new DownloadVideoAsMp3Command { Title = title });
                var searchResult = await _mediator.Send(new SearchSpotifyStructureCommand { Mp3Path = mp3Path });

                mp3PathResults.Add(mp3Path);
                searchResults.Add(searchResult);
            }

            var calculatedComplete = CalculateCompletionRate(searchResults);
            CheckAndCreateLogFile(mp3PathResults.ToString(), calculatedComplete);
            return Ok(calculatedComplete);
        }

        [HttpGet("GetYouTubeMusicPlaylistTitles")]
        public async Task<IActionResult> GetYouTubeMusicPlaylistTitles(string playlistId)
        {
            var request = new GetYouTubeMusicPlaylistTitlesQuery { PlaylistId = playlistId };
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("GetSpotifyPlaylistTitles")]
        public async Task<IActionResult> GetSpotifyPlaylistTitles(string playlistId)
        {
            var request = new GetSpotifyPlaylistTitlesQuery { PlaylistId = playlistId };
            var response = await _mediator.Send(request);
            return Ok(response);
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