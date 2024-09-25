using Moq;
using Songerr.Application.Application.Command;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Tests.UnitTests.ApplicationTests.ControllerTests;

public class DownloadVideoAsMp3HandlerTests
{
    private readonly DownloadVideoAsMp3Handler _handler;
    private readonly Mock<ISongerrService> _songerrServiceMock;

    public DownloadVideoAsMp3HandlerTests()
    {
        _songerrServiceMock = new Mock<ISongerrService>();
        _handler = new DownloadVideoAsMp3Handler(_songerrServiceMock.Object);
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

        var command = new DownloadVideoAsMp3Command { Url = url };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedSong, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenUrlIsNull()
    {
        // Arrange
        var command = new DownloadVideoAsMp3Command { Url = null };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}