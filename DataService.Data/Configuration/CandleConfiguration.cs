using DataService.Contracts.Models.Enums;
using DataService.Data.Convertors;
using DataService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataService.Data.Configuration;

/// <summary>
/// Конфигурация таблицы свечей
/// </summary>
public class CandleConfiguration : IEntityTypeConfiguration<Candle>
{
    public void Configure(EntityTypeBuilder<Candle> builder)
    {
        var loadTypeConverter = new ValueConverter<LoadType, string>(
            x => EnumConverters.ToEnumString(x),
            x => EnumConverters.ToEnum<LoadType>(x),
            new ConverterMappingHints(size: 20, unicode: false));
        
        var candleIntervalConverter = new ValueConverter<Interval, string>(
            x => EnumConverters.ToEnumString(x),
            x => EnumConverters.ToEnum<Interval>(x),
            new ConverterMappingHints(size: 50, unicode: false));
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Open).IsRequired();
        builder.Property(x => x.High).IsRequired();
        builder.Property(x => x.Low).IsRequired();
        builder.Property(x => x.Close).IsRequired();
        builder.Property(x => x.Volume).IsRequired();
        builder.Property(x => x.Time).IsRequired();
        builder.Property(x => x.Interval).HasConversion(candleIntervalConverter).IsRequired();
        builder.Property(x => x.LoadType).HasConversion(loadTypeConverter).IsRequired();

        builder.HasOne(x => x.Share)
            .WithMany(x => x.Candles)
            .HasForeignKey(x => x.ShareId);
    }
}