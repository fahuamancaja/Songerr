using Moq;
using Songerr.Application.Application.Command;
using Songerr.Domain.Interfaces;
using Songerr.Domain.Models;

namespace Songerr.Tests.UnitTests.ApplicationTests.ControllerTests;

public class DownloadVideoAsAudioFileHandlerTests
{
    private readonly DownloadVideoAsAudioFileHandler _handler;
    private readonly Mock<ISongerrService> _songerrServiceMock;

    public DownloadVideoAsAudioFileHandlerTests()
    {
        _songerrServiceMock = new Mock<ISongerrService>();
        _handler = new DownloadVideoAsAudioFileHandler(_songerrServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSongModel_WhenUrlIsValid()
    {
        // Arrange
        var url = "valid_url";
        var expectedSong = new SongModel { Id = "song1", Title = "Song 1" };

        _songerrServiceMock
            .Setup(service => service.SongerrSongService(url))
            .ReturnsAsync(expectedSong);

        var command = new DownloadVideoAsAudioFileCommand { Url = url };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedSong, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenUrlIsNull()
    {
        // Arrange
        var command = new DownloadVideoAsAudioFileCommand { Url = null };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}