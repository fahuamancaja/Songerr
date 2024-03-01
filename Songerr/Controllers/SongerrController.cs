using MediatR;
using Microsoft.AspNetCore.Mvc;
using Songerr.Application.Command;
using Songerr.Application.Query;
using Songerr.Models;
using Songerr.Services;
using System;
using System.Collections.Generic;
using System.IO;


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
            var mp3PathResults = new List<SongModel>();
            foreach (var url in input.Titles)
            {
                var mp3Path = await _mediator.Send(new DownloadVideoAsMp3Command { Url = url });

                mp3PathResults.Add(mp3Path);
            }

            var response = $"Completed {mp3PathResults.Count}";
            return Ok(response);
        }

        [HttpGet("DownloadPlaylistSongs")]
        public async Task<IActionResult> DownloadPlaylistSongs(string playlistId)
        {
            var request = new DownloadPlaylistSongsCommand { PlaylistId = playlistId };
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
    }
}
