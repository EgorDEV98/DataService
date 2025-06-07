using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;

namespace DataService.Data.Common;

/// <summary>
/// Enum to String for DB converter
/// </summary>
public static class EnumConverters
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, string>> EnumToStringMap = new();
    private static readonly ConcurrentDictionary<Type, Dictionary<string, string>> StringToEnumMap = new();
    
    public static string ToEnumString(System.Enum value)
    {
        var enumType = value.GetType();

        if (!EnumToStringMap.TryGetValue(enumType, out var map))
        {
            map = System.Enum.GetNames(enumType)
                .ToDictionary(
                    name => name,
                    name =>
                    {
                        var field = enumType.GetField(name);
                        var attr = field?.GetCustomAttribute<EnumMemberAttribute>();
                        return attr?.Value ?? name;
                    });

            EnumToStringMap[enumType] = map;
        }

        var enumName = System.Enum.GetName(enumType, value);
        if (enumName == null || !map.TryGetValue(enumName, out var result))
            throw new ArgumentException($"Не удалось найти строковое представление для {value}");

        return result;
    }

    public static T ToEnum<T>(string str) where T : struct, System.Enum
    {
        var enumType = typeof(T);

        if (!StringToEnumMap.TryGetValue(enumType, out var map))
        {
            map = System.Enum.GetNames(enumType)
                .ToDictionary(
                    name =>
                    {
                        var field = enumType.GetField(name);
                        var attr = field?.GetCustomAttribute<EnumMemberAttribute>();
                        return attr?.Value ?? name;
                    },
                    name => name);

            StringToEnumMap[enumType] = map;
        }

        if (!map.TryGetValue(str, out var enumName))
            throw new ArgumentException($"Значение '{str}' не найдено в перечислении {enumType.Name}");

        return System.Enum.Parse<T>(enumName);
    }
}