using UrlShortener.WebApi.Entities.Shorten;
using UrlShortener.WebApi.Helpers;
using UrlShortener.WebApi.Services.Interfaces;

namespace UrlShortener.WebApi.Services;

public class UrlShortenerService(
    ILogger<UrlShortenerService> logger,
    IHttpContextAccessor httpContextAccessor) : IUrlShortenerService
{
    private static readonly IList<ShortenEntity> ShortenedUrls = new List<ShortenEntity>();


    public Uri ShortenUrl(string longUrl)
    {
        ArgumentNullException.ThrowIfNull(longUrl);

        if (!(Uri.TryCreate(longUrl, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme ==  Uri.UriSchemeHttps)))
        {
            logger.LogWarning("Invalid URL provided for shortening: {Url}", longUrl);
            throw new ArgumentException("Invalid URL to shorten.", nameof(longUrl));
        }

        var baseUrl = GetBaseUrl();

        var shorten = ShortenedUrls.FirstOrDefault(x => x.LongUrl == uri.ToString());
        if (shorten is not null)
        {
            logger.LogInformation("URL already shortened: {Url}", longUrl);
            return new Uri(baseUrl, shorten.ShortCode);
        }

        var id = ShortenedUrls.Count + 15_000_000;
        var newUrl = Base62Helper.Encode(id);
        var entity = new ShortenEntity(uri.ToString(), id, newUrl);
        ShortenedUrls.Add(entity);
        logger.LogInformation("URL shortened: {Url} to {ShortUrl}", longUrl, newUrl);
        return new Uri(baseUrl, newUrl);
    }

    public Uri? GetLongUrl(string shortCode)
    {
        ArgumentNullException.ThrowIfNull(shortCode);

        var hashId = Base62Helper.Decode(shortCode);
        var entity = ShortenedUrls.FirstOrDefault(x => x.HashId == hashId);
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