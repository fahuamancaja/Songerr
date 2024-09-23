using MediatR;
using Songerr.Domain.Services;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Application.Application.Command;

public class DownloadVideoAsMp3Command : IRequest<SongModel>
{
    public string? Url { get; set; }
}

public class DownloadVideoAsMp3Handler(ISongerrService songerrService)
    : IRequestHandler<DownloadVideoAsMp3Command, SongModel>
{
    public async Task<SongModel> Handle(DownloadVideoAsMp3Command request, CancellationToken cancellationToken)
    {
        if (request.Url != null) return (await songerrService.SongerrSongService(request.Url))!;
        return null!;
    }
}