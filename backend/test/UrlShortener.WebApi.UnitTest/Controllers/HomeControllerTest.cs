using Microsoft.Extensions.Logging;
using Moq;
using UrlShortener.WebApi.Controllers;
using UrlShortener.WebApi.Services.Interfaces;

namespace UrlShortener.WebApi.UnitTest.Controllers;

[Trait("HomeController", "Controller Unit")]
public class HomeControllerTest
{
    private readonly Mock<ILogger<HomeController>> _logger = new();
    private readonly Mock<IUrlShortenerService> _shortenerService = new();
    private readonly HomeController _controller;

    public HomeControllerTest()
    {
        var logger = _logger.Object;
        var shortenerService = _shortenerService.Object;
        _controller = new HomeController(logger, shortenerService);
    }

    /// <summary>
    /// Verifica que ao fornecer um short code válido o controlador retorna um redirecionamento para a URL longa correspondente.
    /// </summary>
    [Theory]
    [InlineData("abc123", "http://example.com")]
    [InlineData("xyz789", "https://www.test.com/page")]
    [InlineData("1a2b3c", "http://localhost/resource")]
    public void Index_ValidShortUrl_ReturnsRedirect(string shortUrl, string returnUrl)
    {
        // Arrange
        var longUrl = new Uri(returnUrl);
        _shortenerService.Setup(s => s.GetLongUrl(shortUrl)).Returns(longUrl);

        // Act
        var result = _controller.Index(shortUrl);

        // Assert
        var redirectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectResult>(result);
        Assert.Equal(longUrl.ToString(), redirectResult.Url);
    }

    /// <summary>
    /// Verifica que quando o short code não existe o resultado é NotFound.
    /// </summary>
    [Fact]
    public void Index_InvalidShortUrl_ReturnsNotFound()
    {
        // Arrange
        var shortUrl = "nonexistent";
        _shortenerService.Setup(s => s.GetLongUrl(shortUrl)).Returns((Uri?)null);

        // Act
        var result = _controller.Index(shortUrl);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }

    /// <summary>
    /// Verifica que uma exceção do serviço resulta em erro 500 (Internal Server Error).
    /// </summary>
    [Fact]
    public void Index_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var shortUrl = "errorUrl";
        _shortenerService.Setup(s => s.GetLongUrl(shortUrl)).Throws(new Exception("Service error"));

        // Act
        var result = _controller.Index(shortUrl);

        // Assert
        var statusCodeResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }

    /// <summary>
    /// Verifica que ao encurtar uma URL válida o controlador retorna Created com a URL encurtada.
    /// </summary>
    [Theory]
    [InlineData("http://example.com", "http://short.ly/abc123")]
    [InlineData("https://www.test.com/page", "http://short.ly/xyz789")]
    [InlineData("http://localhost/resource", "http://short.ly/1a2b3c")]
    public void Shorten_ValidUrl_ReturnsCreated(string url, string returnUrl)
    {
        // Arrange
        var shortenedUrl = new Uri(returnUrl);
        _shortenerService.Setup(s => s.ShortenUrl(url)).Returns(shortenedUrl);

        // Act
        var result = _controller.Shorten(url);

        // Assert
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedResult>(result);
        Assert.Equal(shortenedUrl, createdResult.Value);
    }

    /// <summary>
    /// Verifica que quando o serviço lança exceção no encurtamento, o controlador retorna erro 500.
    /// </summary>
    [Fact]
    public void Shorten_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        const string url = "http://error.com";
        _shortenerService.Setup(s => s.ShortenUrl(url)).Throws(new Exception("Service error"));

        // Act
        var result = _controller.Shorten(url);

        // Assert
        var statusCodeResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}