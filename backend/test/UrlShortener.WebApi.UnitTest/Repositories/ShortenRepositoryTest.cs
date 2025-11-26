using UrlShortener.WebApi.Entities.Shorten;
using UrlShortener.WebApi.Repositories;

namespace UrlShortener.WebApi.UnitTest.Repositories;

[Trait("Repository", "Unit")]
public class ShortenRepositoryTest
{
    private readonly ShortenRepository _repository = new();
    
    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        const string longUrl = "longUrl";
        const string shortCode = "shortUrl";
        const long hashId = 123;
        var entity = new ShortenEntity(longUrl, hashId, shortCode);
        
        // Act
        await _repository.AddAsync(entity);
        
        // Assert
        var retrievedEntity = await _repository.GetByShortCodeAsync(shortCode);
        Assert.NotNull(retrievedEntity);
        Assert.Equal(longUrl, retrievedEntity!.LongUrl);
        Assert.Equal(hashId, retrievedEntity.HashId);
        Assert.Equal(shortCode, retrievedEntity.ShortCode);
    }

    [Fact]
    public async Task GetByLongUrlAsync_ShouldReturnEntity_WhenExists()
    {
        // Arrange
        const string longUrl = "existingLongUrl";
        const string shortCode = "existingShortUrl";
        const long hashId = 456;
        var entity = new ShortenEntity(longUrl, hashId, shortCode);
        await _repository.AddAsync(entity);

        // Act
        var retrievedEntity = await _repository.GetByLongUrlAsync(longUrl);

        // Assert
        Assert.NotNull(retrievedEntity);
        Assert.Equal(longUrl, retrievedEntity!.LongUrl);
        Assert.Equal(hashId, retrievedEntity.HashId);
        Assert.Equal(shortCode, retrievedEntity.ShortCode);
    }

    [Fact]
    public async Task GetByLongUrlAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var retrievedEntity = await _repository.GetByLongUrlAsync("nonExistingLongUrl");

        // Assert
        Assert.Null(retrievedEntity);
    }

    [Fact]
    public async Task GetNextIdAsync_ShouldReturnCorrectNextId()
    {
        // Arrange
        var initialId = await _repository.GetNextIdAsync();
        const string longUrl = "newLongUrl";
        const string shortCode = "newShortUrl";
        const long hashId = 789;
        var entity = new ShortenEntity(longUrl, hashId, shortCode);
        await _repository.AddAsync(entity);

        // Act
        var nextId = await _repository.GetNextIdAsync();

        // Assert
        Assert.Equal(initialId + 1, nextId);
    }
}