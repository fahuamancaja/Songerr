using AutoFixture.Xunit2;
using Moq;
using Serilog;
using Songerr.Domain.Factories;
using Songerr.Domain.Services;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.PayloadModels;
using Songerr.Tests.AutoDataAttributes;
using Xunit;

namespace Songerr.Tests
{
    public class SongerrServiceTests
    {
        private readonly Mock<IYoutubeDlService> _youtubeDlServiceMock;
        private readonly Mock<IParserService> _parserServiceMock;
        private readonly Mock<IMusicSearchService> _musicSearchServiceMock;
        private readonly SongerrService _songerrService;

        public SongerrServiceTests()
        {
            _youtubeDlServiceMock = new Mock<IYoutubeDlService>();
            _parserServiceMock = new Mock<IParserService>();
            _musicSearchServiceMock = new Mock<IMusicSearchService>();

            _songerrService = new SongerrService(
                _youtubeDlServiceMock.Object,
                _parserServiceMock.Object,
                _musicSearchServiceMock.Object
            );
        }

        [Theory]
        [CustomAutoData]
        public async Task SongerrSongService_ShouldProcessCommandsSuccessfully(string videoId)
        {
            // Arrange
            _parserServiceMock.Setup(x => x.ParseVideoUrl(It.IsAny<SongModel>()));
            _youtubeDlServiceMock.Setup(x => x.GetSongMetadata(It.IsAny<SongModel>()));
            _musicSearchServiceMock.Setup(x => x.SearchSpotifyMetadata(It.IsAny<SongModel>()));
            _youtubeDlServiceMock.Setup(x => x.DownloadAudioFile(It.IsAny<SongModel>()));
            _parserServiceMock.Setup(x => x.MoveFileToCorrectLocation(It.IsAny<SongModel>()));
            _parserServiceMock.Setup(x => x.AddMetaDataToFile(It.IsAny<SongModel>()));

            // Act
            var result = await _songerrService.SongerrSongService(videoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(videoId, result.Id);
            _parserServiceMock.Verify(x => x.ParseVideoUrl(It.IsAny<SongModel>()), Times.Once);
            _youtubeDlServiceMock.Verify(x => x.GetSongMetadata(It.IsAny<SongModel>()), Times.Once);
            _musicSearchServiceMock.Verify(x => x.SearchSpotifyMetadata(It.IsAny<SongModel>()), Times.Once);
            _youtubeDlServiceMock.Verify(x => x.DownloadAudioFile(It.IsAny<SongModel>()), Times.Once);
            _parserServiceMock.Verify(x => x.MoveFileToCorrectLocation(It.IsAny<SongModel>()), Times.Once);
            _parserServiceMock.Verify(x => x.AddMetaDataToFile(It.IsAny<SongModel>()), Times.Once);
        }

        [Theory]
        [CustomAutoData]
        public async Task SongerrPlaylistService_ShouldProcessCommandsSuccessfully(SongModel songModel)
        {
            // Arrange
            _youtubeDlServiceMock.Setup(x => x.DownloadAudioFile(It.IsAny<SongModel>()));
            _parserServiceMock.Setup(x => x.MoveFileToCorrectLocation(It.IsAny<SongModel>()));
            _parserServiceMock.Setup(x => x.AddMetaDataToFile(It.IsAny<SongModel>()));
            _musicSearchServiceMock.Setup(x => x.SearchSpotifyMetadata(It.IsAny<SongModel>()));

            // Act
            var result = await _songerrService.SongerrPlaylistService(songModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(songModel.Id, result.Id);
            _musicSearchServiceMock.Verify(x => x.SearchSpotifyMetadata(It.IsAny<SongModel>()), Times.Once);
            _youtubeDlServiceMock.Verify(x => x.DownloadAudioFile(It.IsAny<SongModel>()), Times.Once);
            _parserServiceMock.Verify(x => x.MoveFileToCorrectLocation(It.IsAny<SongModel>()), Times.Once);
            _parserServiceMock.Verify(x => x.AddMetaDataToFile(It.IsAny<SongModel>()), Times.Once);
        }

        [Theory]
        [CustomAutoData]
        public async Task SongerrSongService_WhenVideoIdIsNull_ShouldThrowArgumentNullException(string videoId)
        {
            // Arrange
            _parserServiceMock.Setup(x => x.ParseVideoUrl(It.IsAny<SongModel>())).Callback<SongModel>(model => model.Id = null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _songerrService.SongerrSongService(videoId));
            Assert.Equal("Id", exception.ParamName);
        }
    }
}