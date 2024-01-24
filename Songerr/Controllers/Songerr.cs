using Microsoft.AspNetCore.Mvc;
using Songerr.Services;

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

        [HttpGet("{title}")]
        public async Task<IActionResult> Get(string title)
        {
            var mp3Path = await _songerrService.DownloadFirstVideoAsMp3(title);
            return Ok(new { Mp3Path = mp3Path });
        }
    }
}
