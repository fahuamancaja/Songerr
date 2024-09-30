using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Songerr.Application.Application.Command;
using Songerr.Domain.Models;

namespace Songerr.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongerrController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] SongInput songInput)
    {
        Log.Information($"Received request to convert video URL: {songInput.Url}");

        var songModel = await mediator.Send(new DownloadVideoAsAudioFileCommand { Url = songInput.Url })
            .ConfigureAwait(false);
        Log.Information($"Successfully converted video URL. Audio file path: {songModel.FilePath}");

        return Ok($"Completed {songModel.Title}");
    }

    [HttpGet("DownloadPlaylistSongs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadPlaylistSongs(string playlistId)
    {
        Log.Information($"Received request to convert Playlist Id: {playlistId}");

        return Ok($"Completed:{(await mediator.Send(new DownloadPlaylistSongsCommand { PlaylistId = playlistId })
            .ConfigureAwait(false)).Count}");
    }
}