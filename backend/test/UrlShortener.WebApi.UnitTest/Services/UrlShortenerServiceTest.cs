using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using UrlShortener.WebApi.Services;

namespace UrlShortener.WebApi.UnitTest.Services;

[Trait("UrlShortenerService", "Service Unit")]
public class UrlShortenerServiceTest
{
    private readonly UrlShortenerService _service;
    private readonly Mock<ILogger<UrlShortenerService>> _logger = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();

    public UrlShortenerServiceTest()
    {
        var httpContextAccessor = _httpContextAccessor.Object;
        var logger = _logger.Object;
        _service = new UrlShortenerService(logger, httpContextAccessor);
    }

    /// <summary>
    /// Deve lançar uma exceção ao tentar encurtar uma URL inválida.
    /// </summary>
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
    
    /// <summary>
    /// Deve lançar uma exceção ao tentar encurtar uma URL nula.
    /// </summary>
    [Fact]
    public void ShortenUrl_InvalidUrlFormat_ReturnsException()
    {
        // Act 
        var exception = Assert.Throws<ArgumentNullException>(() => _service.ShortenUrl(null));
        
        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'longUrl')", exception.Message);
    }
    
    /// <summary>
    /// Deve lançar uma exceção ao tentar encurtar uma URL quando o contexto HTTP é inválido.
    /// </summary>
    [Theory]
    [InlineData("http://example.com")]
    [InlineData("https://www.test.com")]
    public void ShortenUrl_HttpContextInvalid_ThrowsException(string url)
    {
        // Arrange
        _httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _service.ShortenUrl(url));
        Assert.Equal("HTTP context is not available.", exception.Message);
    }
    
    /// <summary>
    /// Deve retornar uma nova URL encurtada ao fornecer uma URL válida.
    /// </summary>
    [Theory]
    [InlineData("http://example.com/some/long/url")]
    [InlineData("https://www.test.com/path/to/resource?query=param")]
    [InlineData("http://localhost:8080/page")]
    public void ShortenUrl_ValidUrlFormat_ReturnsNewUrl(string url)
    {
        // Arrange
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Scheme = "http",
                Host = new HostString("localhost", 5000)
            }
        };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _service.ShortenUrl(url);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("http://localhost:5000/", result.ToString());
    }
    
    /// <summary>
    /// Deve retornar a mesma URL encurtada ao chamar múltiplas vezes com a mesma URL longa.
    /// </summary>
    [Theory]
    [InlineData("http://example.com/some/long/url")]
    [InlineData("https://www.test.com/path/to/resource?query=param")]
    [InlineData("http://localhost:8080/page")]
    public void ShortenUrl_ValidUrlFormat_ReturnsSameUrlOnMultipleCalls(string url)
    {
        // Arrange
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Scheme = "http",
                Host = new HostString("localhost", 5000)
            }
        };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var firstResult = _service.ShortenUrl(url);
        var secondResult = _service.ShortenUrl(url);

        // Assert
        Assert.Equal(firstResult, secondResult);
    }
    
    
    /// <summary>
    /// Deve retornar null ao tentar obter uma URL longa para um short code inexistente.
    /// </summary>
    [Fact]
    public void GetLongUrl_NonExistentShortCode_ReturnsNull()
    {
        // Arrange
        var nonExistentShortCode = "nonexistent";

        // Act
        var result = _service.GetLongUrl(nonExistentShortCode);

        // Assert
        Assert.Null(result);
    }
    
    /// <summary>
    /// Deve lançar uma exceção ao tentar obter uma URL longa com um short code nulo.
    /// </summary>
    [Fact]
    public void GetLongUrl_NullShortCode_ThrowsException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _service.GetLongUrl(null!));
        Assert.Equal("Value cannot be null. (Parameter 'shortCode')", exception.Message);
    }

    /// <summary>
    /// Deve retornar a URL longa correta ao fornecer um short code existente.
    /// </summary>
    [Theory]
    [InlineData("http://example.com/some/long/url")]
    [InlineData("https://www.test.com/path/to/resource?query=param")]
    [InlineData("http://localhost:8080/page")]
    public void GetLongUrl_ExistingShortCode_ReturnsLongUrl(string longUrl)
    {
        // Arrange
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Scheme = "http",
                Host = new HostString("localhost", 5000)
            }
        };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var shortenedUri = _service.ShortenUrl(longUrl);
        var shortCode = shortenedUri.Segments.Last();

        // Act
        var result = _service.GetLongUrl(shortCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(longUrl, result.ToString());
    }
}