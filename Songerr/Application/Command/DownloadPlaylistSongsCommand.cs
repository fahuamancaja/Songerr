using MediatR;
using Microsoft.AspNetCore.Mvc;
using Songerr.Models;
using Songerr.Services;

namespace Songerr.Application.Command
{
    // Define the query
    public class DownloadPlaylistSongsCommand : IRequest<List<SongModel>>
    {
        public string PlaylistId { get; set; }
    }

    // Define the handler
    public class DownloadPlaylistSongsHandler : IRequestHandler<DownloadPlaylistSongsCommand, List<SongModel>>
    {
        private readonly IPlaylistRetriever _playlistRetrieverService;

        public DownloadPlaylistSongsHandler(IPlaylistRetriever playlistRetrieverService)
        {
            _playlistRetrieverService = playlistRetrieverService;
        }

        public async Task<List<SongModel>> Handle(DownloadPlaylistSongsCommand request, CancellationToken cancellationToken)
        {
            return await _playlistRetrieverService.DownloadPlaylistSongs(request.PlaylistId);
        }
    }

}
