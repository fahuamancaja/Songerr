using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Songerr.Application.Middleware;

namespace Songerr.Tests.UnitTests.ApplicationTests.MiddlewareTests;

public class GlobalExceptionHandlerTests
{
    private HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/test";
        context.Response.Body = new MemoryStream();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        });
        context.RequestServices = serviceCollection.BuildServiceProvider();

        return context;
    }

    private async Task<T> GetResponseBodyAs<T>(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        using (var reader = new StreamReader(response.Body))
        {
            var responseBody = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<T>(responseBody);
        }
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnTrue_AndLogError()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new ArgumentException("Test exception");
        var handler = new GlobalExceptionHandler();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.TryHandleAsync(httpContext, exception, cancellationToken);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);

        var problemDetails = await GetResponseBodyAs<ProblemDetails>(httpContext.Response);
        Assert.Equal("Test exception", problemDetails.Title);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldSet500StatusCodeForUnknownException()
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = new Exception("Unknown exception");
        var handler = new GlobalExceptionHandler();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.TryHandleAsync(httpContext, exception, cancellationToken);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);

        var problemDetails = await GetResponseBodyAs<ProblemDetails>(httpContext.Response);
        Assert.Equal("Unknown exception", problemDetails.Title);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
    }

    [Theory]
    [InlineData(typeof(ArgumentException), StatusCodes.Status400BadRequest)]
    [InlineData(typeof(UnauthorizedAccessException), StatusCodes.Status401Unauthorized)]
    [InlineData(typeof(KeyNotFoundException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(FileNotFoundException), StatusCodes.Status404NotFound)]
    [InlineData(typeof(NullReferenceException), StatusCodes.Status500InternalServerError)]
    [InlineData(typeof(DivideByZeroException), StatusCodes.Status500InternalServerError)]
    [InlineData(typeof(NotImplementedException), StatusCodes.Status501NotImplemented)]
    public async Task TryHandleAsync_ShouldReturnCorrectStatusCode_ForDifferentExceptions(Type exceptionType,
        int expectedStatusCode)
    {
        // Arrange
        var httpContext = CreateHttpContext();
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Test exception");
        var handler = new GlobalExceptionHandler();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.TryHandleAsync(httpContext, exception, cancellationToken);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedStatusCode, httpContext.Response.StatusCode);

        var problemDetails = await GetResponseBodyAs<ProblemDetails>(httpContext.Response);
        Assert.Equal("Test exception", problemDetails.Title);
        Assert.Equal(expectedStatusCode, problemDetails.Status);
    }
}