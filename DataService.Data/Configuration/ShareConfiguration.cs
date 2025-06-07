using DataService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataService.Data.Configuration;

/// <summary>
/// Конфигурация таблицы акций
/// </summary>
public class ShareConfiguration : IEntityTypeConfiguration<Share>
{
    public void Configure(EntityTypeBuilder<Share> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Figi).IsRequired();
        builder.Property(x => x.Ticker).IsRequired();
        builder.Property(x => x.First1MinCandleDate).IsRequired();
        builder.Property(x => x.First1DayCandleDate).IsRequired();
        builder.Property(x => x.IsEnableToLoad).HasDefaultValue(false).IsRequired();
        
        builder.HasIndex(x => x.Figi).IsUnique();
        builder.HasIndex(x => x.Ticker).IsUnique();
    }
}