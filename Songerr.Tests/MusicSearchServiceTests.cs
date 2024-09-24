using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using Moq;
using Xunit;
using Songerr.Domain.Services;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.Models;
using Songerr.Infrastructure.PayloadModels;
using System.Collections.Generic;

namespace Songerr.Tests
{
    public class MusicSearchServiceTests
    {
        private readonly Mock<ISpotifyClientSearch> _spotifyClientSearchMock;
        private readonly MusicSearchService _musicSearchService;
        private readonly IFixture _fixture;

        public MusicSearchServiceTests()
        {
            _spotifyClientSearchMock = new Mock<ISpotifyClientSearch>();
            _musicSearchService = new MusicSearchService(_spotifyClientSearchMock.Object);
            _fixture = new Fixture();
        }

        [Theory, AutoData]
        public async Task SearchSpotifyMetaData_WhenTokenIsNull_ShouldThrowException(SongModel songModel)
        {
            // Arrange
            _spotifyClientSearchMock.Setup(s => s.GetSpotifyAccessTokenAsync()).ReturnsAsync(null as string);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _musicSearchService.SearchSpotifyMetaData(songModel));
        }

        [Theory, AutoData]
        public async Task SearchSpotifyMetaData_WhenSpotifySongItemsIsNull_ShouldNotThrowException(SongModel songModel)
        {
            // Arrange
            _spotifyClientSearchMock.Setup(s => s.GetSpotifyAccessTokenAsync()).ReturnsAsync("access_token");
            _spotifyClientSearchMock.Setup(s => s.GetSpotifyMetaData(songModel, "access_token")).ReturnsAsync((SpotifyResults?)null);

            // Act & Assert
            await _musicSearchService.SearchSpotifyMetaData(songModel);
        }

        [Theory, AutoData]
        public async Task SearchSpotifyMetaData_WhenFirstItemIsNull_ShouldNotSetAlbumName([Frozen] SongModel songModel)
        {
            // Arrange
            songModel.Album = "InitialAlbum";
            
            _spotifyClientSearchMock.Setup(s => s.GetSpotifyAccessTokenAsync()).ReturnsAsync("access_token");

            var spotifyResults = _fixture
                .Build<SpotifyResults>()
                .With(s => s.tracks, new Tracks { items = new List<Item>() })
                .Create();

            _spotifyClientSearchMock.Setup(s => s.GetSpotifyMetaData(songModel, "access_token")).ReturnsAsync(spotifyResults);

            // Act
            await _musicSearchService.SearchSpotifyMetaData(songModel);

            // Assert
            Assert.Equal("InitialAlbum", songModel.Album);
        }

        [Theory, AutoData]
        public async Task SearchSpotifyMetaData_WhenFirstItemIsNotNull_ShouldSetCleanedAlbumName([Frozen] SongModel songModel)
        {
            // Arrange
            _spotifyClientSearchMock.Setup(s => s.GetSpotifyAccessTokenAsync()).ReturnsAsync("access_token");

            var spotifyResults = _fixture
                .Build<SpotifyResults>()
                .With(s => s.tracks, new Tracks
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
                })
                .Create();

            _spotifyClientSearchMock.Setup(s => s.GetSpotifyMetaData(songModel, "access_token")).ReturnsAsync(spotifyResults);

            // Act
            await _musicSearchService.SearchSpotifyMetaData(songModel);

            // Assert
            Assert.Equal("Test Album", songModel.Album);
        }
    }
}