﻿using AutoMapper;
using Moq;
using Songerr.Domain.Interfaces;
using Songerr.Domain.Models;
using Songerr.Domain.Services;
using Songerr.Tests.AutoDataAttributes;
using YoutubeExplode.Playlists;

namespace Songerr.Tests.UnitTests.DomainTests;

public class PlaylistServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly PlaylistService _playlistService;
    private readonly Mock<ISongerrService> _songerrServiceMock;
    private readonly Mock<IYoutubeDlClient> _youtubeDlClientMock;

    public PlaylistServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _youtubeDlClientMock = new Mock<IYoutubeDlClient>();
        _songerrServiceMock = new Mock<ISongerrService>();
        _playlistService =
            new PlaylistService(_mapperMock.Object, _youtubeDlClientMock.Object, _songerrServiceMock.Object);
    }

    [Theory]
    [CustomAutoData]
    public async Task DownloadPlaylistAudioFiles_WhenPlaylistIdIsValid_ShouldReturnSongModels(
        string playlistId, List<PlaylistVideo> playListModels, SongModel songModel)
    {
        // Arrange
        _youtubeDlClientMock
            .Setup(x => x.GetPlaylistMetadata(It.IsAny<string>()))
            .ReturnsAsync(playListModels);

        _mapperMock
            .Setup(x => x.Map<SongModel>(It.IsAny<PlaylistVideo>()))
            .Returns(songModel);

        _songerrServiceMock
            .Setup(x => x.SongerrPlaylistService(It.IsAny<SongModel>()));

        // Act
        var result = await _playlistService.DownloadPlaylistAudioFiles(playlistId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(playListModels.Count, result!.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DownloadPlaylistAudioFiles_WhenPlaylistIdIsNullOrWhitespace_ShouldReturnNull(string? playlistId)
    {
        // Act
        var result = await _playlistService.DownloadPlaylistAudioFiles(playlistId);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [CustomAutoData]
    public async Task DownloadPlaylistAudioFiles_WhenNoSongsReturned_ShouldReturnNull(string playlistId)
    {
        // Arrange
        _youtubeDlClientMock
            .Setup(x => x.GetPlaylistMetadata(It.IsAny<string>()))
            .ReturnsAsync((List<PlaylistVideo>?)null);

        // Act
        var result = await _playlistService.DownloadPlaylistAudioFiles(playlistId);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [CustomAutoData]
    public async Task DownloadPlaylistAudioFiles_WhenMapperReturnsNull_ShouldReturnEmptyList(string playlistId,
        List<PlaylistVideo> playListModels)
    {
        // Arrange
        _youtubeDlClientMock
            .Setup(x => x.GetPlaylistMetadata(It.IsAny<string>()))
            .ReturnsAsync(playListModels);

        _mapperMock
            .Setup(x => x.Map<SongModel>(It.IsAny<PlaylistVideo>()))
            .Returns((SongModel?)null);

        // Act
        var result = await _playlistService.DownloadPlaylistAudioFiles(playlistId);

        // Assert
        Assert.Empty(result!);
    }
}