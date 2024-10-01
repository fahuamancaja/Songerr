using Moq;
using Songerr.Application.Application.Command;
using Songerr.Domain.Interfaces;
using Songerr.Domain.Models;
using Songerr.Tests.AutoDataAttributes;
using TagLib.Riff;

namespace Songerr.Tests.UnitTests.ApplicationTests.CommandTests;

public class DownloadVideoAsAudioFileHandlerTests
{
    private readonly DownloadVideoAsAudioFileHandler _handler;
    private readonly Mock<ISongerrService> _songerrServiceMock;

    public DownloadVideoAsAudioFileHandlerTests()
    {
        _songerrServiceMock = new Mock<ISongerrService>();
        _handler = new DownloadVideoAsAudioFileHandler(_songerrServiceMock.Object);
    }

    [Theory]
    [CustomAutoData]
    public async Task Handle_ReturnsSongModel_WhenUrlIsValid(string url, SongModel expectedSong)
    {
        // Arrange
        _songerrServiceMock
            .Setup(service => service.SongerrSongService(url))
            .ReturnsAsync(expectedSong);

        var command = new DownloadVideoAsAudioFileCommand { Url = url };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedSong, result);
    }

    [Theory]
    [InlineData(null)]
    public async Task Handle_ReturnsNull_WhenUrlIsNull(string? url)
    {
        // Arrange
        var command = new DownloadVideoAsAudioFileCommand { Url = url };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}