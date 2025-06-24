using DataService.Data.Configuration;
using DataService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataService.Data;

/// <summary>
/// Контекст БД
/// </summary>
public class PostgresDbContext : DbContext
{
    /// <summary>
    /// Акции
    /// </summary>
    public DbSet<Share> Shares { get; set; }
    
    /// <summary>
    /// Свечи
    /// </summary>
    public DbSet<Candle> Candles { get; set; }
    
    /// <summary>
    /// Расписания торгов
    /// </summary>
    public DbSet<Scheduler> Schedulers { get; set; }
    
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ShareConfiguration());
        modelBuilder.ApplyConfiguration(new CandleConfiguration());
        modelBuilder.ApplyConfiguration(new SchedulerConfiguration());
    }
}