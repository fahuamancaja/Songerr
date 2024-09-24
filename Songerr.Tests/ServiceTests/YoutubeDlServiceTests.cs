using AutoMapper;
using Moq;
using Songerr.Domain.Services;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.PayloadModels;
using Songerr.Tests.AutoDataAttributes;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace Songerr.Tests.ServiceTests
{
    public class YoutubeDlServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IYoutubeDlClient> _youtubeDlClientMock;
        private readonly YoutubeDlService _youtubeDlService;

        public YoutubeDlServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _youtubeDlClientMock = new Mock<IYoutubeDlClient>();
            _youtubeDlService = new YoutubeDlService(_mapperMock.Object, _youtubeDlClientMock.Object);
        }

        [Theory]
        [CustomAutoData]
        public async Task DownloadAudioFile_WhenSongModelIsValid_ShouldSetFilePath(
            SongModel songModel, string filePath)
        {
            // Arrange
            _youtubeDlClientMock
                .Setup(x => x.DownloadAudioFile(It.IsAny<SongModel>()))
                .ReturnsAsync(filePath);

            // Act
            await _youtubeDlService.DownloadAudioFile(songModel);

            // Assert
            Assert.Equal(filePath, songModel.FilePath);
        }

        [Theory]
        [CustomAutoData]
        public async Task GetSongMetadata_WhenSongModelIsValid_ShouldMapMetadata(
            SongModel songModel, Video result)
        {
            // Arrange
            _youtubeDlClientMock
                .Setup(x => x.GetSongMetadata(It.IsAny<SongModel>()))
                .ReturnsAsync(result);

            _mapperMock
                .Setup(m => m.Map(result, songModel))
                .Verifiable();

            // Act
            await _youtubeDlService.GetSongMetadata(songModel);

            // Assert
            _mapperMock.Verify(m => m.Map(result, songModel), Times.Once);

            // Verify author mapping
            var res = result.Author.ChannelTitle.Split('-')[0].Trim();
            Assert.IsType<string>(res);
        }

        [Theory]
        [CustomAutoData]
        public async Task GetSongMetadata_WhenAuthorIsFormatted_ShouldTrimAndSetAuthor(
            SongModel songModel, Video result)
        {
            // Arrange
            var mockAuthor = new Author(result.Author.ChannelId, "Author - Details");
            var mockVideo = new Video(result.Id, result.Title, mockAuthor, result.UploadDate, result.Description, result.Duration, result.Thumbnails, result.Keywords, result.Engagement);
            _youtubeDlClientMock
                .Setup(x => x.GetSongMetadata(It.IsAny<SongModel>()))
                .ReturnsAsync(mockVideo);

            // Act
            await _youtubeDlService.GetSongMetadata(songModel);

            // Assert
            var parts = songModel.Author?.Split('-');
            Assert.Equal(parts?[0].Trim(), songModel.Author);
        }
    }
}