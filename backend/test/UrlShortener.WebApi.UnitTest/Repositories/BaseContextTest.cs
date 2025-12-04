using Microsoft.EntityFrameworkCore;
using UrlShortener.WebApi.Infra.Context;

namespace UrlShortener.WebApi.UnitTest.Repositories;

public abstract class BaseContextTest
{
    protected static UrlShortenerContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<UrlShortenerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new UrlShortenerContext(options);
    }
}