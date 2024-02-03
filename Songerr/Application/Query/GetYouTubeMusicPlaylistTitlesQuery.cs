using MediatR;
using Microsoft.AspNetCore.Mvc;
using Songerr.Services;

namespace Songerr.Application.Query
{
    // Define the query
    public class GetYouTubeMusicPlaylistTitlesQuery : IRequest<IEnumerable<string>>
    {
        public string PlaylistId { get; set; }
    }

    // Define the handler
    public class GetPlaylistTitlesHandler : IRequestHandler<GetYouTubeMusicPlaylistTitlesQuery, IEnumerable<string>>
    {
        private readonly IPlaylistRetriever _playlistRetrieverService;

        public GetPlaylistTitlesHandler(IPlaylistRetriever playlistRetrieverService)
        {
            _playlistRetrieverService = playlistRetrieverService;
        }

        public async Task<IEnumerable<string>> Handle(GetYouTubeMusicPlaylistTitlesQuery request, CancellationToken cancellationToken)
        {
            return await _playlistRetrieverService.GetPlaylistTitlesAsync(request.PlaylistId);
        }
    }

}
