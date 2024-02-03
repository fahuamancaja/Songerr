using MediatR;
using Microsoft.AspNetCore.Mvc;
using Songerr.Services;

namespace Songerr.Application.Query
{
    // Define the query
    public class GetSpotifyPlaylistTitlesQuery : IRequest<IEnumerable<string>>
    {
        public string PlaylistId { get; set; }
    }

    // Define the handler
    public class GetSpotifyPlaylistTitlesHandler : IRequestHandler<GetSpotifyPlaylistTitlesQuery, IEnumerable<string>>
    {
        private readonly ISpotifyService _spotifyService;

        public GetSpotifyPlaylistTitlesHandler(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        public async Task<IEnumerable<string>> Handle(GetSpotifyPlaylistTitlesQuery request, CancellationToken cancellationToken)
        {
            return await _spotifyService.GetSongTitlesAndArtistsAsync(request.PlaylistId);
        }
    }
}
