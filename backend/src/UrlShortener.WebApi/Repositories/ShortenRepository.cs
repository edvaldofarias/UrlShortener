using UrlShortener.WebApi.Entities.Shorten;
using UrlShortener.WebApi.Repositories.Interfaces;

namespace UrlShortener.WebApi.Repositories;

public class ShortenRepository : IShortenRepository
{
    private static readonly IList<ShortenEntity> ShortenedUrls = [];

    public Task AddAsync(ShortenEntity shorten, CancellationToken cancellationToken = default)
    {
        ShortenedUrls.Add(shorten);
        return Task.CompletedTask;
    }

    public Task<ShortenEntity?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        var entity = ShortenedUrls.FirstOrDefault(u => u.ShortCode == shortCode);
        return Task.FromResult(entity);
    }

    public Task<ShortenEntity?> GetByLongUrlAsync(string longUrl, CancellationToken cancellationToken = default)
    {
        var entity = ShortenedUrls.FirstOrDefault(u => u.LongUrl == longUrl);
        return Task.FromResult(entity);
    }

    public Task<long> GetNextIdAsync(CancellationToken cancellationToken = default)
    {
        var nextId = ShortenedUrls.Count + 1;
        return Task.FromResult((long)nextId);
    }
}