using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using UrlShortener.WebApi.Entities.Shorten;
using UrlShortener.WebApi.Infra.Repositories.Interfaces;
using UrlShortener.WebApi.Services;

namespace UrlShortener.WebApi.UnitTest.Services;

[Trait("UrlShortenerService", "Service Unit")]
public class UrlShortenerServiceTest
{
    private readonly UrlShortenerService _service;
    private readonly Mock<ILogger<UrlShortenerService>> _logger = new();
    private readonly Mock<ISequenceRepository> _sequenceRepository = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<IShortenRepository> _shortenRepository = new();
    private readonly Mock<IMemoryCache> _memoryCache = new();

    public UrlShortenerServiceTest()
    {
        var httpContextAccessor = _httpContextAccessor.Object;
        var logger = _logger.Object;
        var repository = _shortenRepository.Object;
        var sequenceRepository = _sequenceRepository.Object;
        var memoryCache = _memoryCache.Object;
        _service = new UrlShortenerService(
            logger, 
            httpContextAccessor, 
            repository, 
            sequenceRepository, 
            memoryCache);
    }

    /// <summary>
    /// Deve lançar uma exceção ao tentar encurtar uma URL inválida.
    /// </summary>
    [Theory]
    [InlineData("/shorten")]
    [InlineData("/api/shorten")]
    [InlineData("")]
    [InlineData("htp:/invalid-url")]
    public async Task ShortenUrl_InvalidUrl_ReturnsException(string invalidUrl)
    {
        // Act 
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.ShortenUrlAsync(invalidUrl));
        
        // Assert
        Assert.Equal("Invalid URL to shorten. (Parameter 'longUrl')", exception.Message);
    }
    
    /// <summary>
    /// Deve lançar uma exceção ao tentar encurtar uma URL quando o contexto HTTP é inválido.
    /// </summary>
    [Theory]
    [InlineData("http://example.com")]
    [InlineData("https://www.test.com")]
    public async Task ShortenUrl_HttpContextInvalid_ThrowsException(string url)
    {
        // Arrange
        _httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ShortenUrlAsync(url));
        Assert.Equal("HTTP context is not available.", exception.Message);
    }
    
    /// <summary>
    /// Deve retornar uma nova URL encurtada ao fornecer uma URL válida.
    /// </summary>
    [Theory]
    [InlineData("http://example.com/some/long/url")]
    [InlineData("https://www.test.com/path/to/resource?query=param")]
    [InlineData("http://localhost:8080/page")]
    [InlineData("https://www.test.com")]
    public async Task ShortenUrl_ValidUrlFormat_ReturnsNewUrl(string url)
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
        _shortenRepository.Setup(x => x.GetByLongUrlAsync(url)).ReturnsAsync(default(Shorten?));

        // Act
        var result = await _service.ShortenUrlAsync(url);

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
    [InlineData("https://www.test.com")]
    public async Task ShortenUrl_ValidUrlFormat_ReturnsSameUrlOnMultipleCalls(string url)
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
        var urlComplete = new Uri(url).ToString();
        
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        _shortenRepository.Setup(x => x.GetByLongUrlAsync(urlComplete)).ReturnsAsync(default(Shorten?));

        // Act
        var firstResult = await _service.ShortenUrlAsync(url);
        _shortenRepository.Setup(x => x.GetByLongUrlAsync(urlComplete)).ReturnsAsync(new Shorten(urlComplete, 1, firstResult.AbsolutePath));
        var secondResult = await _service.ShortenUrlAsync(url);

        // Assert
        Assert.Equal(firstResult, secondResult);
    }
    
    
    /// <summary>
    /// Deve retornar null ao tentar obter uma URL longa para um short code inexistente.
    /// </summary>
    [Fact]
    public async Task GetLongUrl_NonExistentShortCode_ReturnsNull()
    {
        // Arrange
        var nonExistentShortCode = "nonexistent";

        // Act
        var result = await _service.GetLongUrlAsync(nonExistentShortCode);
        

        // Assert
        Assert.Null(result);
    }
    
    /// <summary>
    /// Deve lançar uma exceção ao tentar obter uma URL longa com um short code nulo.
    /// </summary>
    [Fact]
    public async Task GetLongUrl_NullShortCode_ThrowsException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetLongUrlAsync(null!));
        Assert.Equal("Value cannot be null. (Parameter 'shortCode')", exception.Message);
    }

    /// <summary>
    /// Deve retornar a URL longa correta ao fornecer um short code existente.
    /// </summary>
    [Theory]
    [InlineData("http://example.com/Adasd23")]
    [InlineData("https://www.test.com/cba321")] 
    [InlineData("http://localhost:8080/abc123")]
    [InlineData("https://www.test.com/abc123")]
    public async Task GetLongUrl_ExistingShortCode_ReturnsLongUrl(string longUrl)
    {        // Arrange
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Scheme = "http",
                Host = new HostString("localhost", 5000)
            }
        };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        object? shorten = null;
        _memoryCache.Setup(x => x.TryGetValue(It.IsAny<long>(), out shorten))
            .Returns(false);
        var shortenedUri = await _service.ShortenUrlAsync(longUrl);
        var shortCode = shortenedUri.Segments[^1];
        _shortenRepository.Setup(x => x.GetByShortCodeAsync(shortCode)).ReturnsAsync(new Shorten(longUrl, 1, shortCode));

        // Act
        var result = await _service.GetLongUrlAsync(shortCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(longUrl, result.ToString());
    }
}