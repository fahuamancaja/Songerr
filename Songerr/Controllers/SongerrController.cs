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
            var mp3PathResults = new List<string>();
            var searchResults = new List<string>();
            foreach (var title in input.Titles)
            {
                var mp3Path = await _mediator.Send(new DownloadVideoAsMp3Command { Title = title });
                var searchResult = await _mediator.Send(new SearchSpotifyStructureCommand { Mp3Path = mp3Path });

                mp3PathResults.Add(mp3Path);
                searchResults.Add(searchResult);
            }

            var fullyCompleted = CheckAndCreateLogFile(mp3PathResults, searchResults);
            var response = $"{fullyCompleted} / {searchResults.Count}";
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


        private int CheckAndCreateLogFile(List<string> mp3Path, List<string> searchResult)
        {
            var completedCounter = 0;
            string logFolderPath = "Logs";
            string logFileName = "mp3.log";
            string logFilePath = Path.Combine(logFolderPath, logFileName);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            if (!System.IO.File.Exists(logFilePath))
            {
                using (StreamWriter sw = System.IO.File.CreateText(logFilePath))
                {
                    foreach (var song in mp3Path)
                    {
                        sw.WriteLine($"Original MP3 Path: {song}");
                    }

                    foreach (var newSong in searchResult)
                    {
                        if (newSong == string.Empty)
                        {
                            sw.WriteLine($"No metadata found song");
                        }
                        if (newSong == "file does not exist")
                        {
                            sw.WriteLine($"{newSong}");
                        }
                        else
                        {
                            sw.WriteLine($"New song added with metadata: {newSong}");
                            completedCounter++;
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter sw = System.IO.File.AppendText(logFilePath))
                {
                    foreach (var song in mp3Path)
                    {
                        sw.WriteLine($"Original MP3 Path: {song}");
                    }

                    foreach (var newSong in searchResult)
                    {
                        if (newSong == string.Empty)
                        {
                            sw.WriteLine($"No metadata found for: {newSong}");
                        }
                        if (newSong == "file does not exist")
                        {
                            sw.WriteLine($"Cannot find file source for: {newSong}");
                        }
                        else
                        {
                            sw.Write($"New song added with metadata: {newSong}");
                            completedCounter++;
                        }
                    }
                }
            }
            return completedCounter;
        }

    }
}
