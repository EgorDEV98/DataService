using DataService.Contracts.Models.Enums;
using DataService.Data.Convertors;
using DataService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataService.Data.Configuration;

/// <summary>
/// Конфигурация таблицы акций
/// </summary>
public class ShareConfiguration : IEntityTypeConfiguration<Share>
{
    public void Configure(EntityTypeBuilder<Share> builder)
    {
        var loadStatusConverter = new ValueConverter<LoadStatus, string>(
            x => EnumConverters.ToEnumString(x),
            x => EnumConverters.ToEnum<LoadStatus>(x),
            new ConverterMappingHints(size: 20, unicode: false));
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Ticker).IsRequired();
        builder.Property(x => x.Figi).IsRequired();
        builder.Property(x => x.ClassCode).IsRequired();
        builder.Property(x => x.Lot).IsRequired();
        builder.Property(x => x.Currency).IsRequired();
        builder.Property(x => x.ShortEnabledFlag).IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.CountryOfRisk).IsRequired();
        builder.Property(x => x.CountryOfRiskName).IsRequired();
        builder.Property(x => x.Sector).IsRequired();
        builder.Property(x => x.DivYieldFlag).IsRequired();
        builder.Property(x => x.MinPriceIncrement).IsRequired();
        builder.Property(x => x.WeekendFlag).IsRequired();
        builder.Property(x => x.First1MinCandleDate).IsRequired();
        builder.Property(x => x.First1DayCandleDate).IsRequired();
        builder.Property(x => x.CandleLoadStatus).HasConversion(loadStatusConverter).IsRequired();

        builder.HasIndex(x => x.Ticker).IsUnique();
        
        builder.HasMany(x => x.Candles)
            .WithOne(x => x.Share)
            .HasForeignKey(x => x.ShareId);
    }
}