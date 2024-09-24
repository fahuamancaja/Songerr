﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Songerr.Application.Application.Command;
using Songerr.Domain.Models;

namespace Songerr.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongerrController : ControllerBase
{
    private readonly IMediator _mediator;

    public SongerrController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SongInput songInput)
    {
        Log.Information($"Received request to convert video URL: {songInput.Url}");

        var songModel = await _mediator.Send(new DownloadVideoAsMp3Command { Url = songInput.Url })
            .ConfigureAwait(false);
        Log.Information($"Successfully converted video URL. MP3 file path: {songModel.FilePath}");

        return Ok($"Completed {songModel.Title}");
    }

    [HttpGet("DownloadPlaylistSongs")]
    public async Task<IActionResult> DownloadPlaylistSongs(string playlistId)
    {
        Log.Information($"Received request to convert Playlist Id: {playlistId}");

        return Ok($"Completed:{(await _mediator.Send(new DownloadPlaylistSongsCommand { PlaylistId = playlistId })
            .ConfigureAwait(false)).Count}");
    }
}