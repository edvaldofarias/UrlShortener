using Microsoft.EntityFrameworkCore;
using UrlShortener.WebApi.Entities.Shorten;
using UrlShortener.WebApi.Infra.Context;
using UrlShortener.WebApi.Infra.Repositories.Interfaces;

namespace UrlShortener.WebApi.Infra.Repositories;

public class ShortenRepository(UrlShortenerContext context) : IShortenRepository
{
    public async Task AddAsync(Shorten shorten, CancellationToken cancellationToken = default)
    {
        await context.Shorten.AddAsync(shorten, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Shorten?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        var shorten = await context.Shorten.Where(x => x.ShortCode == shortCode).FirstOrDefaultAsync(cancellationToken);
        return shorten;
    }

    public async Task<Shorten?> GetByLongUrlAsync(string longUrl, CancellationToken cancellationToken = default)
    {
        var sorten = await context.Shorten.Where(x => x.LongUrl == longUrl).FirstOrDefaultAsync(cancellationToken);
        return sorten;
    }
}