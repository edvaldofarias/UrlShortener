namespace UrlShortener.WebApi.Entities.Shorten;

public sealed class ShortenEntity(string longUrl, long hashId, string shortCode) : EntityBase
{
    public string LongUrl { get; private set; } = longUrl;

    public long HashId { get; private set; } = hashId;

    public string ShortCode { get; private set; } = shortCode;
}