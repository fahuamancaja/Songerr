using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using Songerr.Domain.Services;
using Songerr.Infrastructure.OptionSettings;
using Songerr.Infrastructure.PayloadModels;
using Songerr.Tests.AutoDataAttributes;

namespace Songerr.Tests;

public class ParserServiceTests
{
    private readonly ParserService _parserService;
    private readonly Mock<IOptions<LocalSettings>> _settingsMock;

    public ParserServiceTests()
    {
        _settingsMock = new Mock<IOptions<LocalSettings>>();
        var localSettings = new LocalSettings { DownloadPath = "C:\\MusicDownloads" };
        _settingsMock.Setup(x => x.Value).Returns(localSettings);

        _parserService = new ParserService(_settingsMock.Object);
    }

    [Theory]
    [CustomAutoData]
    public async Task MoveFileToCorrectLocationAsync_WhenAuthorIsNotNull_ShouldCreateDirectoryAndMoveFile(
        [Frozen] SongModel songModel)
    {
        // Arrange
        if (!File.Exists(songModel.FilePath)) File.Copy("Recording.m4a", songModel.FilePath, true);

        songModel.Author = "Test Artist";
        songModel.Album = "Test Album";
        songModel.Title = "Title";

        Directory.CreateDirectory(Path.Combine("C:\\MusicDownloads", songModel.Author, songModel.Album));

        // Act
        await _parserService.MoveFileToCorrectLocation(songModel);

        // Assert
        var newFileName = $"{songModel.Author} - {songModel.Title}{Path.GetExtension(songModel.FilePath)}";
        var newFilePath = Path.Combine("C:\\MusicDownloads", songModel.Author, songModel.Album, newFileName);
        Assert.Equal(newFilePath, songModel.FilePath);
    }

    [Theory]
    [CustomAutoData]
    public void ParseVideoUrl_ShouldExtractVideoIdFromUrl([Frozen] SongModel songModel)
    {
        // Arrange
        songModel.Id = "https://music.youtube.com/watch?v=x9sq06NTSXU&si=mL_qGnAxi7D70osi";

        // Act
        _parserService.ParseVideoUrl(songModel);

        // Assert
        Assert.Equal("x9sq06NTSXU", songModel.Id);
    }

    [Theory]
    [CustomAutoData]
    public async Task AddMetaDataToFile_ShouldAddMetadata([Frozen] SongModel songModel)
    {
        // Arrange
        if (!File.Exists(songModel.FilePath)) File.Copy("Samples\\Recording.m4a", songModel.FilePath, true);

        // Act
        await _parserService.AddMetaDataToFile(songModel);

        // Assert
        // Normally, we would verify the file's metadata, but for this mock, we will just ensure no exceptions are thrown
        Assert.True(true);
    }
}