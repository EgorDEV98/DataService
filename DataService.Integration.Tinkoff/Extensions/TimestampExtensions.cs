using Google.Protobuf.WellKnownTypes;

namespace DataService.Integration.Tinkoff.Extensions;

/// <summary>
/// Расширения для Timestamp
/// </summary>
public static class TimestampExtensions
{
    /// <summary>
    /// Преобразует UTC-время в московское (UTC+3) без использования TimeZoneInfo.
    /// </summary>
    /// <param name="timestamp">Протобуф Timestamp в UTC</param>
    /// <returns>DateTimeOffset с временной зоной UTC+3</returns>
    public static DateTimeOffset ToMoscowTime(this Timestamp timestamp)
    {
        if (timestamp == null)
            throw new ArgumentNullException(nameof(timestamp));

        return timestamp.ToDateTimeOffset().AddHours(3);
    }
}