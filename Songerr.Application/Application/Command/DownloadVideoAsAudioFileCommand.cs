using MediatR;
using Songerr.Domain.Interfaces;
using Songerr.Domain.Models;

namespace Songerr.Application.Application.Command;

public class DownloadVideoAsAudioFileCommand : IRequest<SongModel>
{
    public string? Url { get; set; }
}

public class DownloadVideoAsAudioFileHandler(ISongerrService songerrService)
    : IRequestHandler<DownloadVideoAsAudioFileCommand, SongModel>
{
    public async Task<SongModel> Handle(DownloadVideoAsAudioFileCommand request, CancellationToken cancellationToken)
    {
        if (request.Url != null) return (await songerrService.SongerrSongService(request.Url))!;
        return null!;
    }
}