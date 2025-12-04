using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.WebApi.Entities.Shorten;

namespace UrlShortener.WebApi.Infra.Configurations;

[ExcludeFromCodeCoverage]
public class ShortenConfiguration : IEntityTypeConfiguration<Shorten>
{
    public void Configure(EntityTypeBuilder<Shorten> builder)
    {
        builder.ToTable("Shorten");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.HashId)
            .IsRequired();

        builder.Property(x => x.LongUrl)
            .IsRequired();

        builder.Property(x => x.ShortCode)
            .HasMaxLength(8)
            .IsRequired();
        
        builder.HasIndex(x => x.HashId)
            .IsUnique();
        
        builder.HasIndex(x => x.ShortCode)
            .IsUnique();
    }
}