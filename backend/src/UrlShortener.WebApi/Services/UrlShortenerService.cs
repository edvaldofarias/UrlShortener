using UrlShortener.WebApi.Entities.Shorten;
using UrlShortener.WebApi.Helpers;
using UrlShortener.WebApi.Infra.Repositories.Interfaces;
using UrlShortener.WebApi.Services.Interfaces;

namespace UrlShortener.WebApi.Services;

public class UrlShortenerService(
    ILogger<UrlShortenerService> logger,
    IHttpContextAccessor httpContextAccessor,
    IShortenRepository shortenRepository,
    ISequenceRepository sequenceRepository) : IUrlShortenerService
{
    public async Task<Uri> ShortenUrlAsync(string longUrl)
    {
        ArgumentNullException.ThrowIfNull(longUrl);

        if (!(Uri.TryCreate(longUrl, UriKind.Absolute, out var uri) &&
              (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
        {
            logger.LogWarning("Invalid URL provided for shortening: {Url}", longUrl);
            throw new ArgumentException("Invalid URL to shorten.", nameof(longUrl));
        }

        var baseUrl = GetBaseUrl();
        var urlComplete = uri.ToString();

        var shorten = await shortenRepository.GetByLongUrlAsync(urlComplete);
        if (shorten is not null)
        {
            logger.LogInformation("URL already shortened: {Url}", urlComplete);
            return new Uri(baseUrl, shorten.ShortCode);
        }

        const string sequenceName = "Shorten_HashId_Seq";
        var nextId = await sequenceRepository.GetNextSequenceValueAsync(sequenceName);
        var newUrl = Base62Helper.Encode(nextId);
        var entity = new Shorten(urlComplete, nextId, newUrl);
        
        await shortenRepository.AddAsync(entity);
        logger.LogInformation("URL shortened: {Url} to {ShortUrl}", urlComplete, newUrl);
        return new Uri(baseUrl, newUrl);
    }

    public async Task<Uri?> GetLongUrlAsync(string shortCode)
    {
        ArgumentNullException.ThrowIfNull(shortCode);

        var hashId = Base62Helper.Decode(shortCode);
        var entity = await shortenRepository.GetByShortCodeAsync(shortCode);
        if (entity != null)
        {
            logger.LogInformation("Redirecting short URL {ShortUrl} to {LongUrl}", hashId, entity.LongUrl);
            return new Uri(entity.LongUrl);
        }

        logger.LogWarning("Short URL not found: {ShortUrl}", shortCode);
        return null;
    }

    private Uri GetBaseUrl()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        if (request is null)
        {
            logger.LogWarning("HTTP context not available when shortening URL: {Url}", httpContextAccessor.HttpContext);
            throw new InvalidOperationException("HTTP context is not available.");
        }

        return new Uri($"{request.Scheme}://{request.Host}{request.PathBase}/");
    }
}