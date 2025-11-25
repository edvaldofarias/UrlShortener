namespace UrlShortener.WebApi.Services.Interfaces;

public interface IUrlShortenerService
{
    public Task<Uri> ShortenUrlAsync(string longUrl);

    public Task<Uri?> GetLongUrlAsync(string shortCode);
}