namespace UrlShortener.WebApi.Entities;

public abstract class EntityBase
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset? UpdatedAt { get; private set; } = null;
    
    public void SetUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}