using MediatR;
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
    public async Task<IActionResult> Post([FromBody] SongInput payload)
    {
        Log.Information($"Received request to convert video URL: {payload.Url}");

        var songModel = await _mediator.Send(new DownloadVideoAsMp3Command { Url = payload.Url }).ConfigureAwait(false);
        Log.Information($"Successfully converted video URL. MP3 file path: {songModel.FilePath}");
        
        return Ok($"Completed {songModel.Title}");
    }

    [HttpGet("DownloadPlaylistSongs")]
    public async Task<IActionResult> DownloadPlaylistSongs(string playlistId)
    {
        var response = await _mediator.Send(new DownloadPlaylistSongsCommand { PlaylistId = playlistId }).ConfigureAwait(false);
        return Ok($"Completed: {response.Count}");
    }
}