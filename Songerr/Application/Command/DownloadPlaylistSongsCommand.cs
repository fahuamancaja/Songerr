using MediatR;
using Microsoft.AspNetCore.Mvc;
using Songerr.Services;

namespace Songerr.Application.Command
{
    // Define the query
    public class DownloadPlaylistSongsCommand : IRequest<IEnumerable<string>>
    {
        public string PlaylistId { get; set; }
    }

    // Define the handler
    public class DownloadPlaylistSongsHandler : IRequestHandler<DownloadPlaylistSongsCommand, IEnumerable<string>>
    {
        private readonly IPlaylistRetriever _playlistRetrieverService;

        public DownloadPlaylistSongsHandler(IPlaylistRetriever playlistRetrieverService)
        {
            _playlistRetrieverService = playlistRetrieverService;
        }

        public async Task<IEnumerable<string>> Handle(DownloadPlaylistSongsCommand request, CancellationToken cancellationToken)
        {
            return await _playlistRetrieverService.DownloadPlaylistSongs(request.PlaylistId);
        }
    }

}
