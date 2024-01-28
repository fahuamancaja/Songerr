using Microsoft.AspNetCore.Mvc;
using Songerr.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Songerr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Songerr : ControllerBase
    {
        private readonly ISongerrService _songerrService;
        private readonly IMusicSearchService _musicSearchService;

        public Songerr(ISongerrService songerrService, IMusicSearchService musicSearchService)
        {
            _songerrService = songerrService;
            _musicSearchService = musicSearchService;
        }

        public class SongInput
        {
            public List<string> Titles { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SongInput input)
        {
            var results = new List<string>();

            foreach (var title in input.Titles)
            {
                var mp3Path = await _songerrService.DownloadFirstVideoAsMp3(title);
                results.Add(mp3Path);
            }

            return Ok(results);
        }

        [HttpGet("GetSongInfo")]
        public async Task<IActionResult> GetSongInfo(string songName)
        {
            var songInfo = await _musicSearchService.GetSongInfoAsync(songName);

            if (songInfo == null)
            {
                return NotFound($"No song found with the name {songName}.");
            }

            return Ok(songInfo);
        }
    }
}
