using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Songerr.Application.Application.Command;
using Songerr.Application.Controllers;
using Songerr.Domain.Models;
using Songerr.Infrastructure.PayloadModels;
using Songerr.Tests.AutoDataAttributes;

namespace Songerr.Tests.UnitTests.ControllerTests;

public class SongerrControllerTests
{
    private readonly SongerrController _controller;
    private readonly Mock<IMediator> _mediatorMock;

    public SongerrControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new SongerrController(_mediatorMock.Object);
    }

    [Theory]
    [CustomAutoData]
    public async Task Post_ValidSongInput_ShouldReturnOkResult(
        SongInput songInput, SongModel songModel)
    {
        // Arrange
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<DownloadVideoAsMp3Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(songModel);

        // Act
        var result = await _controller.Post(songInput);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"Completed {songModel.Title}", okResult.Value);
    }

    [Theory]
    [CustomAutoData]
    public async Task DownloadPlaylistSongs_ValidPlaylistId_ShouldReturnOkResult(
        string playlistId, List<SongModel> songModels)
    {
        // Arrange
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<DownloadPlaylistSongsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(songModels);

        // Act
        var result = await _controller.DownloadPlaylistSongs(playlistId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"Completed:{songModels.Count}", okResult.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DownloadPlaylistSongs_InvalidPlaylistId_ShouldReturnOkResult(
        string playlistId)
    {
        // Arrange
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<DownloadPlaylistSongsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SongModel>());

        // Act
        var result = await _controller.DownloadPlaylistSongs(playlistId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Completed:0", okResult.Value);
    }
}