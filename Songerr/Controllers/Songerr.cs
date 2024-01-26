using Microsoft.AspNetCore.Mvc;
using Songerr.Services;
using System.Collections.Generic;

namespace Songerr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Songerr : ControllerBase
    {
        private readonly ISongerrService _songerrService;

        public Songerr(ISongerrService songerrService)
        {
            _songerrService = songerrService;
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
    }
}
