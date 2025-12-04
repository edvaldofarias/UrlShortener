using UrlShortener.WebApi.Entities.Shorten;
using UrlShortener.WebApi.Infra.Repositories;

namespace UrlShortener.WebApi.UnitTest.Repositories;

[Trait("Repository", "Unit")]
public class ShortenRepositoryTest : BaseContextTest
{
    private readonly ShortenRepository _repository;

    public ShortenRepositoryTest()
    {
        var context = CreateInMemoryContext();
        _repository = new ShortenRepository(context);
    }
    
    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        const string longUrl = "longUrl";
        const string shortCode = "shortUrl";
        const long hashId = 123;
        var entity = new Shorten(longUrl, hashId, shortCode);
        
        // Act
        await _repository.AddAsync(entity);
        
        // Assert
        var retrievedEntity = await _repository.GetByShortCodeAsync(shortCode);
        Assert.NotNull(retrievedEntity);
        Assert.Equal(longUrl, retrievedEntity.LongUrl);
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
        var entity = new Shorten(longUrl, hashId, shortCode);
        await _repository.AddAsync(entity);

        // Act
        var retrievedEntity = await _repository.GetByLongUrlAsync(longUrl);

        // Assert
        Assert.NotNull(retrievedEntity);
        Assert.Equal(longUrl, retrievedEntity.LongUrl);
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
}