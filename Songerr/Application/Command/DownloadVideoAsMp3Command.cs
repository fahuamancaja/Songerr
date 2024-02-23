using MediatR;
using Songerr.Services;

namespace Songerr.Application.Command
{
    public class DownloadVideoAsMp3Command : IRequest<string>
    {
        public string Title { get; set; }
    }

    public class DownloadVideoAsMp3Handler : IRequestHandler<DownloadVideoAsMp3Command, string>
    {
        private readonly ISongerrService _songerrService;

        public DownloadVideoAsMp3Handler(ISongerrService songerrService)
        {
            _songerrService = songerrService;
        }

        public async Task<string> Handle(DownloadVideoAsMp3Command request, CancellationToken cancellationToken)
        {
            return await _songerrService.GetSingleMp3BasedOnUrl(request.Title);
        }
    }

}
