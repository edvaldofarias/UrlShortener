namespace UrlShortener.WebApi.Services.Interfaces;

public interface IUrlShortenerService
{
    public Uri ShortenUrl(string longUrl);
    
    public Uri? GetLongUrl(string shortCode);
}