using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using UrlShortener.WebApi.Infra.Context;
using UrlShortener.WebApi.Infra.Repositories;

namespace UrlShortener.WebApi.UnitTest.Repositories;

public class SequenceRepositoryTest : BaseContextTest
{
    [Fact]
    public async Task GetNextSequenceValueAsync_ShouldReturnNextValue()
    {
        // Arrange
        var postgres = new PostgreSqlBuilder()
            .WithDatabase("url_shortener_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await postgres.StartAsync();

        var options = new DbContextOptionsBuilder<UrlShortenerContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        const string sequenceName = "minha_sequencia";
        const string query = $"CREATE SEQUENCE \"{sequenceName}\" START 1 INCREMENT 1;";

        var context = new UrlShortenerContext(options);
        await context.Database.ExecuteSqlRawAsync(query);
        
        var repository = new SequenceRepository(context);
        
        // Act
        var firstValue = await repository.GetNextSequenceValueAsync(sequenceName);
        var secondValue = await repository.GetNextSequenceValueAsync(sequenceName);
        
        // Assert
        Assert.Equal(1, firstValue);
        Assert.Equal(2, secondValue);
        
        await postgres.DisposeAsync();
    }
}