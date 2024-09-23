using Moq;
using Xunit;
using Songerr.Domain.Services;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.Models;
using Songerr.Infrastructure.PayloadModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Songerr.Tests;

public class MusicSearchServiceTests
{
    private readonly Mock<ISpotifyClientSearch> _spotifyClientSearchMock;
    private readonly MusicSearchService _musicSearchService;

    public MusicSearchServiceTests()
    {
        _spotifyClientSearchMock = new Mock<ISpotifyClientSearch>();
        
        _musicSearchService = new MusicSearchService(_spotifyClientSearchMock.Object);
    }

    [Fact]
    public async Task SearchSpotifyMetaData_WhenTokenIsNull_ShouldThrowException()
    {
        // Arrange
        _spotifyClientSearchMock.Setup(s => s.GetSpotifyAccessTokenAsync()).ReturnsAsync(null as string);
        
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _musicSearchService.SearchSpotifyMetaData(new SongModel()));
    }

    [Fact]
    public async Task SearchSpotifyMetaData_WhenSpotifySongItemsIsNull_ShouldNotThrowException()
    {
        // Arrange
        _spotifyClientSearchMock.Setup(s => s.GetSpotifyAccessTokenAsync()).ReturnsAsync("access_token");
        
        _spotifyClientSearchMock.Setup(s => s.GetSpotifyMetaData(It.IsAny<SongModel>(),"access_token")).ReturnsAsync((SpotifyResults?)null);
        
        // Act & Assert
        await _musicSearchService.SearchSpotifyMetaData(new SongModel());
    }

    [Fact]
    public async Task SearchSpotifyMetaData_WhenFirstItemIsNull_ShouldNotSetAlbumName()
    {
        // Arrange
        var songModel = new SongModel
        {
            Album = "InitialAlbum"
        };
        
        _spotifyClientSearchMock.Setup(s => s.GetSpotifyAccessTokenAsync()).ReturnsAsync("access_token");
        
        var spotifyResults = new SpotifyResults
        {
            tracks = new Tracks
            {
                items = new List<Item>()
            }
        };
        
        _spotifyClientSearchMock.Setup(s => s.GetSpotifyMetaData(songModel, "access_token")).ReturnsAsync(spotifyResults);

        // Act
        await _musicSearchService.SearchSpotifyMetaData(songModel);

        // Assert
        Assert.Equal("InitialAlbum", songModel.Album);
    }

    [Fact]
    public async Task SearchSpotifyMetaData_WhenFirstItemIsNotNull_ShouldSetCleanedAlbumName()
    {
        // Arrange
        var songModel = new SongModel();
        
        _spotifyClientSearchMock.Setup(s => s.GetSpotifyAccessTokenAsync()).ReturnsAsync("access_token");
        
        var spotifyResults = new SpotifyResults
        {
            tracks = new Tracks
            {
                items = new List<Item>
                {
                    new Item
                    {
                        album = new Album
                        {
                            name = "Test Album (Deluxe) [Live] {Special Edition}!"
                        }
                    }
                }
            }
        };
        
        _spotifyClientSearchMock.Setup(s => s.GetSpotifyMetaData(songModel, "access_token")).ReturnsAsync(spotifyResults);

        // Act
        await _musicSearchService.SearchSpotifyMetaData(songModel);

        // Assert
        Assert.Equal("Test Album", songModel.Album);
    }
}