using MediatR;
using Songerr.Services;

namespace Songerr.Application.Command
{
    public class SearchSpotifyStructureCommand : IRequest<string>
    {
        public string Mp3Path { get; set; }
    }
    public class SearchSpotifyStructureHandler : IRequestHandler<SearchSpotifyStructureCommand, string>
    {
        private readonly IMusicSearchService _musicSearchService;

        public SearchSpotifyStructureHandler(IMusicSearchService musicSearchService)
        {
            _musicSearchService = musicSearchService;
        }

        public async Task<string> Handle(SearchSpotifyStructureCommand request, CancellationToken cancellationToken)
        {
            return await _musicSearchService.SearchSpotifyStructure(request.Mp3Path);
        }
    }

}
