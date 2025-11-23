using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using UrlShortener.WebApi.Services;

namespace UrlShortener.WebApi.UnitTest.Services;

public class UrlShortenerServiceTest
{
    private readonly UrlShortenerService _service;
    private readonly Mock<ILogger<UrlShortenerService>> _logger = new Mock<ILogger<UrlShortenerService>>();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new Mock<IHttpContextAccessor>();

    public UrlShortenerServiceTest()
    {
        var httpContextAccessor = _httpContextAccessor.Object;
        var logger = _logger.Object;
        _service = new UrlShortenerService(logger, httpContextAccessor);
    }

    [Theory]
    [InlineData("/shorten")]
    [InlineData("/api/shorten")]
    [InlineData("")]
    [InlineData("htp:/invalid-url")]
    public void ShortenUrl_InvalidUrl_ReturnsException(string invalidUrl)
    {
        // Act 
        var exception = Assert.Throws<ArgumentException>(() => _service.ShortenUrl(invalidUrl!));
        
        // Assert
        Assert.Equal("Invalid URL to shorten. (Parameter 'longUrl')", exception.Message);
    }
    
    [Fact]
    public void ShortenUrl_InvalidUrlFormat_ReturnsException()
    {
        // Act 
        var exception = Assert.Throws<ArgumentNullException>(() => _service.ShortenUrl(null));
        
        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'longUrl')", exception.Message);
    }
}