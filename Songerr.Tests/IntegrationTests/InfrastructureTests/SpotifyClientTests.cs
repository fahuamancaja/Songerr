using Flurl.Http;
using Flurl.Http.Testing;
using Microsoft.Extensions.Options;
using Moq;
using Songerr.Infrastructure.ApiClients;
using Songerr.Infrastructure.Models;
using Songerr.Infrastructure.OptionSettings;
using Songerr.Infrastructure.PayloadModels;

namespace Songerr.Tests.IntegrationTests;

public class SpotifyClientTests
{
    private readonly Mock<IOptions<SpotifySettings>> _mockSettings;
    private readonly SpotifyClient _spotifyClient;

    public SpotifyClientTests()
    {
        _mockSettings = new Mock<IOptions<SpotifySettings>>();
        _mockSettings.Setup(s => s.Value).Returns(new SpotifySettings
        {
            ClientId = "testClientId",
            ClientSecret = "testClientSecret"
        });

        _spotifyClient = new SpotifyClient(_mockSettings.Object);
    }

    [Fact]
    public async Task GetSpotifyAccessTokenAsync_ReturnsAccessToken()
    {
        using (var httpTest = new HttpTest())
        {
            // Arrange
            httpTest.RespondWithJson(new { access_token = "testAccessToken" });

            // Act
            var token = await _spotifyClient.GetSpotifyAccessTokenAsync();

            // Assert
            Assert.NotNull(token);
            Assert.Equal("testAccessToken", token);
        }
    }

    [Fact]
    public async Task GetSpotifyMetaData_ReturnsSpotifyResults()
    {
        using (var httpTest = new HttpTest())
        {
            // Arrange
            var songModel = new SongModel { Author = "Test Artist", Title = "Test Track" };
            var expectedResult = new SpotifyResults
            {
                tracks = new Tracks
                {
                    items = new List<Item>
                    {
                        new()
                        {
                            name = "Test Track",
                            artists = new List<Artist>
                            {
                                new() { name = "Test Artist" }
                            },
                            album = new Album { name = "Test Album" }
                        }
                    }
                }
            };
            var accessToken = "testAccessToken";

            httpTest.RespondWithJson(expectedResult);

            // Act
            var result = await _spotifyClient.GetSpotifyMetaData(songModel, accessToken);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.tracks?.items);
            Assert.Equal("Test Track", result.tracks?.items?[0].name);
            Assert.Equal("Test Artist", result.tracks?.items?[0].artists?[0].name);
            Assert.Equal("Test Album", result.tracks?.items?[0].album?.name);
        }
    }

    [Fact]
    public async Task GetSpotifyMetaData_ReturnsNull_WhenResponseIsNotSuccessful()
    {
        using (var httpTest = new HttpTest())
        {
            // Arrange
            var songModel = new SongModel { Author = "Test Artist", Title = "Test Track" };
            var accessToken = "testAccessToken";

            httpTest.RespondWith(status: 400);

            // Act
            SpotifyResults result = null;
            try
            {
                result = await _spotifyClient.GetSpotifyMetaData(songModel, accessToken);
            }
            catch (FlurlHttpException ex)
            {
                // Assert
                Assert.NotNull(ex);
                Assert.Equal(400, ex.Call.Response.StatusCode);
            }

            // Final Assert to confirm result is null due to exception
            Assert.Null(result);
        }
    }
}