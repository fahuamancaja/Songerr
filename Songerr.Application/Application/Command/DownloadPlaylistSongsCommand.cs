using MediatR;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Application.Application.Command;

// Define the query
public class DownloadPlaylistSongsCommand : IRequest<List<SongModel>>
{
    public string? PlaylistId { get; set; }
}

// Define the handler
public class DownloadPlaylistSongsHandler : IRequestHandler<DownloadPlaylistSongsCommand, List<SongModel>>
{
    private readonly IPlaylistService _playlistServiceService;

    public DownloadPlaylistSongsHandler(IPlaylistService playlistServiceService)
    {
        _playlistServiceService = playlistServiceService;
    }

    public async Task<List<SongModel>> Handle(DownloadPlaylistSongsCommand request, CancellationToken cancellationToken)
    {
        return (await _playlistServiceService.DownloadPlaylistAudioFiles(request.PlaylistId))!;
    }
}