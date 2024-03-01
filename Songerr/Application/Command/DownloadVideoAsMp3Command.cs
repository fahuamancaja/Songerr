using MediatR;
using Songerr.Models;
using Songerr.Services;

namespace Songerr.Application.Command
{
    public class DownloadVideoAsMp3Command : IRequest<SongModel>
    {
        public string Url { get; set; }
    }

    public class DownloadVideoAsMp3Handler : IRequestHandler<DownloadVideoAsMp3Command, SongModel>
    {
        private readonly ISongerrService _songerrService;

        public DownloadVideoAsMp3Handler(ISongerrService songerrService)
        {
            _songerrService = songerrService;
        }

        public async Task<SongModel> Handle(DownloadVideoAsMp3Command request, CancellationToken cancellationToken)
        {
            return await _songerrService.GetSingleMp3BasedOnUrl(request.Url);
        }
    }

}
