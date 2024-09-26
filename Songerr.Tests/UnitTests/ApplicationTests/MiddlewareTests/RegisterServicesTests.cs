using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Songerr.Application.Middleware;
using Songerr.Domain.Factories;
using Songerr.Infrastructure.Interfaces;
using Songerr.Infrastructure.OptionSettings;
using YoutubeDLSharp;
using YoutubeExplode;
using Xunit;

namespace Songerr.Tests.UnitTests.ApplicationTests.MiddlewareTests;

public class ServiceCollectionExtensionTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly IServiceCollection _services;

    public ServiceCollectionExtensionTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _services = new ServiceCollection();
    }

    [Fact]
    public void Should_RegisterServicesCorrectly()
    {
        // Arrange
        _mockConfiguration.Setup(config => config.GetSection(It.IsAny<string>()))
            .Returns(new Mock<IConfigurationSection>().Object);

        // Act
        var serviceProvider = _services.RegisterServices(_mockConfiguration.Object).BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IPlaylistService>());
        Assert.NotNull(serviceProvider.GetService<IYoutubeDlClient>());
        Assert.NotNull(serviceProvider.GetService<IMusicSearchService>());
        Assert.NotNull(serviceProvider.GetService<ISongerrService>());
        Assert.NotNull(serviceProvider.GetService<IParserService>());
        Assert.NotNull(serviceProvider.GetService<ISpotifyClientSearch>());
        Assert.NotNull(serviceProvider.GetService<IYoutubeDlService>());
        Assert.NotNull(serviceProvider.GetService<YoutubeDL>());
        Assert.NotNull(serviceProvider.GetService<YoutubeClient>());
        Assert.NotNull(serviceProvider.GetService<ISongProcessingCommand>());
        Assert.NotNull(serviceProvider.GetService<IOptions<YoutubeSettings>>());
        Assert.NotNull(serviceProvider.GetService<IOptions<SpotifySettings>>());
        Assert.NotNull(serviceProvider.GetService<IOptions<SongerrSettings>>());
        Assert.NotNull(serviceProvider.GetService<IOptions<LocalSettings>>());
    }
}