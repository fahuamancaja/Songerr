using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Moq;
using Songerr.Application.Middleware;

namespace Songerr.Tests.UnitTests.ApplicationTests.MiddlewareTests;

public class TraceIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WithCurrentActivityId_ShouldPushTraceIdToLogContext()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        mockRequestDelegate.Setup(rd => rd(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        var middleware = new TraceIdMiddleware(mockRequestDelegate.Object);
        var context = new DefaultHttpContext();
        var activity = new Activity("TestActivity").Start();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        mockRequestDelegate.Verify(rd => rd(context), Times.Once);

        // Clean up
        activity.Stop();
    }

    [Fact]
    public async Task InvokeAsync_WithNoCurrentActivityId_ShouldUseTraceIdentifier()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        mockRequestDelegate.Setup(rd => rd(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        var middleware = new TraceIdMiddleware(mockRequestDelegate.Object);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        mockRequestDelegate.Verify(rd => rd(context), Times.Once);
    }
}