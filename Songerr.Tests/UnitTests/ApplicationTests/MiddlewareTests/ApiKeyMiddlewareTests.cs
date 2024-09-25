using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Songerr.Application;

namespace Songerr.Tests.UnitTests.ApplicationTests.MiddlewareTests;

public class ApiKeyMiddlewareTests
{
    private readonly HttpContext _httpContext;
    private readonly ApiKeyMiddleware _middleware;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<RequestDelegate> _mockNext;

    public ApiKeyMiddlewareTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockNext = new Mock<RequestDelegate>();
        _httpContext = new DefaultHttpContext();
        _middleware = new ApiKeyMiddleware(_mockNext.Object, _mockConfig.Object);
    }

    [Fact]
    public async Task InvokeAsync_NoApiKey_ReturnsBadRequest()
    {
        // Arrange
        _httpContext.Request.Headers.Remove("X-API-Key");

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        _mockNext.Verify(m => m(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_InvalidApiKey_ReturnsUnauthorized()
    {
        // Arrange
        _httpContext.Request.Headers["X-API-Key"] = "invalid-key";
        _mockConfig.Setup(config => config["ApiKeys:MyApiKey"]).Returns("valid-key");

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.Unauthorized, _httpContext.Response.StatusCode);
        _mockNext.Verify(m => m(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_ValidApiKey_CallsNextMiddleware()
    {
        // Arrange
        _httpContext.Request.Headers["X-API-Key"] = "valid-key";
        _mockConfig.Setup(config => config["ApiKeys:MyApiKey"]).Returns("valid-key");

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.NotEqual((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
        Assert.NotEqual((int)HttpStatusCode.Unauthorized, _httpContext.Response.StatusCode);
        _mockNext.Verify(m => m(It.IsAny<HttpContext>()), Times.Once);
    }
}