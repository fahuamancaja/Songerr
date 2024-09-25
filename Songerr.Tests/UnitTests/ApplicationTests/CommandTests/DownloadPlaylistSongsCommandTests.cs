using Moq;
using Songerr.Application.Application.Command;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Tests.UnitTests.ApplicationTests.CommandTests;

public class DownloadPlaylistSongsHandlerTests
{
    private readonly DownloadPlaylistSongsHandler _handler;
    private readonly Mock<IPlaylistService> _playlistServiceMock;

    public DownloadPlaylistSongsHandlerTests()
    {
        _playlistServiceMock = new Mock<IPlaylistService>();
        _handler = new DownloadPlaylistSongsHandler(_playlistServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSongsList_WhenPlaylistIdIsValid()
    {
        // Arrange
        var playlistId = "valid_playlist_id";
        var expectedSongs = new List<SongModel>
        {
            new() { Id = "song1", Title = "Song 1" },
            new() { Id = "song2", Title = "Song 2" }
        };

        _playlistServiceMock
            .Setup(service => service.DownloadPlaylistAudioFiles(playlistId))
            .ReturnsAsync(expectedSongs);

        var command = new DownloadPlaylistSongsCommand { PlaylistId = playlistId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedSongs, result);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenPlaylistIdIsInvalid()
    {
        // Arrange
        var playlistId = "invalid_playlist_id";

        _playlistServiceMock
            .Setup(service => service.DownloadPlaylistAudioFiles(playlistId))
            .ReturnsAsync(new List<SongModel>());

        var command = new DownloadPlaylistSongsCommand { PlaylistId = playlistId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}