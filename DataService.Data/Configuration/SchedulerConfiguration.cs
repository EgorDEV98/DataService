using DataService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataService.Data.Configuration;

/// <summary>
/// Конфигурация таблицы расписание биржи
/// </summary>
public class SchedulerConfiguration : IEntityTypeConfiguration<Scheduler>
{
    public void Configure(EntityTypeBuilder<Scheduler> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.StartTime);
        builder.Property(x => x.EndTime);
        builder.Property(x => x.IsTradingDay).IsRequired();
        builder.Property(x => x.Exchange).IsRequired();

        builder.HasIndex(x => new { x.Exchange, x.StartTime, x.EndTime });
    }
}