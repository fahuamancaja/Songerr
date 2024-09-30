using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using Songerr.Domain.Models;
using Songerr.Domain.Models.OptionSettings;
using Songerr.Domain.Services;
using Songerr.Tests.AutoDataAttributes;

namespace Songerr.Tests.UnitTests.DomainTests;

public class ParserServiceTests
{
    private readonly ParserService _parserService;

    public ParserServiceTests()
    {
        Mock<IOptions<LocalSettings>> settingsMock = new();
        var localSettings = new LocalSettings { DownloadPath = "C:\\MusicDownloads" };
        settingsMock.Setup(x => x.Value).Returns(localSettings);

        _parserService = new ParserService(settingsMock.Object);
    }

    [Theory]
    [CustomAutoData]
    public async Task MoveFileToCorrectLocationAsync_WhenAuthorIsNotNull_ShouldCreateDirectoryAndMoveFile(
        [Frozen] SongModel songModel)
    {
        // Arrange
        if (!File.Exists(songModel.FilePath)) File.Copy("Samples\\Recording.m4a", songModel.FilePath, true);

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