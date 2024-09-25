using AutoMapper;
using Songerr.Application.Middleware;
using Songerr.Infrastructure.PayloadModels;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Should_Map_PlaylistVideo_To_SongModel_Correctly()
    {
        // Arrange
        var playlistVideo = new PlaylistVideo(
            new PlaylistId("PL1"),
            new VideoId("V1"),
            "Test Title (Live) [Official]",
            new Author(new ChannelId("1"), "Test Author (Live) [Official]"),
            new TimeSpan(0, 1, 2),
            new List<Thumbnail>()
        );

        // Act
        var songModel = _mapper.Map<SongModel>(playlistVideo);

        // Assert
        Assert.Equal("PL1", songModel.PlaylistId);
        Assert.Equal("V1", songModel.Id);
        Assert.Equal("Test Title", songModel.Title); // Verify special character removal
        Assert.Equal("Test Author", songModel.Author); // Verify special character removal
    }

    [Fact]
    public void Should_Map_Video_To_SongModel_Correctly()
    {
        // Arrange
        var video = new Video(
            new VideoId("V1"),
            "Video Title (Live) [Official]",
            new Author(new ChannelId("1"), "Video Author (Live) [Official]"),
            DateTimeOffset.Now,
            "Video Description",
            new TimeSpan(0, 3, 45),
            new List<Thumbnail>(),
            new List<string> { "keyword1", "keyword2" },
            new Engagement(100, 10, 5)
        );

        // Act
        var songModel = _mapper.Map<SongModel>(video);

        // Assert
        Assert.Equal("V1", songModel.Id);
        Assert.Equal("Video Title", songModel.Title); // Verify special character removal
        Assert.Equal("Video Author", songModel.Author); // Verify special character removal
    }

    [Theory]
    [InlineData("Test (Live) [Official]", "Test")]
    [InlineData("Author (Live) [Official]", "Author")]
    [InlineData("Title with !Special@ #Chars$", "Title with Special Chars")]
    public void Should_RemoveBracesAndTrailingSpacesAndSpecialChars(string input, string expectedOutput)
    {
        // Act
        var result = MappingProfile.RemoveBracesAndTrailingSpacesAndSpecialChars(input);

        // Assert
        Assert.Equal(expectedOutput, result);
    }
}