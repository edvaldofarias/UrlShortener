using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using UrlShortener.WebApi.Entities.Shorten;

namespace UrlShortener.WebApi.Infra.Context;

[ExcludeFromCodeCoverage]
public class UrlShortenerContext(DbContextOptions<UrlShortenerContext> options) : DbContext(options)
{
    public DbSet<Shorten> Shorten => Set<Shorten>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UrlShortenerContext).Assembly);

        const long start = 15_000_000;
        const int increment = 1;
        const string sequenceName = "Shorten_HashId_Seq";
        CreateSequences(modelBuilder, sequenceName, start, increment);
    }
    
    private static void CreateSequences(ModelBuilder modelBuilder, string name, long start, int increment)
    {
        modelBuilder.HasSequence<long>(name)
            .StartsAt(start)
            .IncrementsBy(increment);
    }
}