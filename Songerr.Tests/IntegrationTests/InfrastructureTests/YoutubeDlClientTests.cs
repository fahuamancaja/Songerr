using Microsoft.Extensions.Options;
using Songerr.Infrastructure.ApiClients;
using Songerr.Infrastructure.OptionSettings;
using Songerr.Infrastructure.PayloadModels;
using YoutubeDLSharp;
using YoutubeExplode;

namespace Songerr.Tests.IntegrationTests.InfrastructureTests;

public class YoutubeDlClientTests
{
    private readonly LocalSettings _settings;
    private readonly YoutubeClient _youtubeClient;
    private readonly YoutubeDL _youtubeDl;
    private readonly YoutubeDlClient _youtubeDlClient;

    public YoutubeDlClientTests()
    {
        _settings = new LocalSettings
        {
            OperatingSystem = "Windows",
            YoutubeDLPath = "/path/to/youtubedl",
            FFmpegPath = "/path/to/ffmpeg"
        };

        _youtubeDl = new YoutubeDL();
        _youtubeClient = new YoutubeClient();
        var options = Options.Create(_settings);

        _youtubeDlClient = new YoutubeDlClient(options, _youtubeDl, _youtubeClient);
    }

    [Fact(Skip = "Integration Tests")]
    public async Task DownloadAudioFile_ValidSongModel_ReturnsFilePath()
    {
        // Arrange
        var songModel = new SongModel { Id = "ValidYoutubeVideoId" };

        // Act
        var result = await _youtubeDlClient.DownloadAudioFile(songModel);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("", result);
    }

    [Fact(Skip = "Integration Tests")]
    public async Task GetSongMetadata_ValidSongModel_ReturnsVideo()
    {
        // Arrange
        var songModel = new SongModel { Id = "xSiLHMBgkjY" };

        // Act
        var result = await _youtubeDlClient.GetSongMetadata(songModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(songModel.Id, result.Id);
        Assert.False(string.IsNullOrEmpty(result.Title));
    }

    [Fact(Skip = "Integration Tests")]
    public async Task GetPlaylistMetadata_ValidPlaylistId_ReturnsPlaylistVideos()
    {
        // Arrange
        var playlistId = "PLGLE04byxYs5wgRt4MBCleQdFVqvW-RBB";

        // Act
        var result = await _youtubeDlClient.GetPlaylistMetadata(playlistId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}