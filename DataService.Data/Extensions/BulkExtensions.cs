using System.Text;
using DataService.Data.Convertors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace DataService.Data.Extensions;

public static class BulkExtensions
{
    /// <summary>
    /// Добавить список сущностей игнорируя дубликаты
    /// </summary>
    /// <param name="context"></param>
    /// <param name="entities"></param>
    /// <param name="batchSize"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task BulkInsertAsync<T>(
        this DbContext context,
        IEnumerable<T> entities,
        int batchSize = 1000,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        // Конвертируем в список для многократного прохода
        var list = entities.ToList();
        if (list.Count == 0) return;

        // Получаем метаданные сущности T
        var entityType = context.Model.FindEntityType(typeof(T));
        if (entityType == null)
            throw new InvalidOperationException($"Тип {typeof(T)} не найден в модели DbContext.");

        // Имя таблицы и схемы (экранируются ниже)
        var schema = entityType.GetSchema();
        var tableName = entityType.GetTableName();
        if (string.IsNullOrWhiteSpace(tableName))
            throw new InvalidOperationException($"Не удалось определить имя таблицы для типа {typeof(T)}.");

        // Определяем идентификатор таблицы для получения имен колонок
        var storeObject = StoreObjectIdentifier.Table(tableName, schema);

        // Выбираем свойства, которые нужно вставлять: не shadow, не PK с автоинкрементом, не concurrency
        var props = entityType.GetProperties()
            .Where(p => !p.IsShadowProperty()
                        && !(p.IsPrimaryKey() && p.ValueGenerated != ValueGenerated.Never && (p.ClrType == typeof(int) || p.ClrType == typeof(long)))
                        && !p.IsConcurrencyToken)
            .ToList();

        // Собираем имена столбцов из EF (в базе данных)
        var columns = props.Select(p => p.GetColumnName(storeObject)).ToList();
        if (columns.Count == 0)
            throw new InvalidOperationException($"Для типа {typeof(T)} не найдены вставляемые столбцы.");

        string tableFullName = string.IsNullOrEmpty(schema)
            ? Quote(tableName)
            : $"{Quote(schema)}.{Quote(tableName)}";
        string columnList = string.Join(", ", columns.Select(Quote));

        // Работаем через соединение Npgsql
        var conn = context.Database.GetDbConnection();
        if (conn is not NpgsqlConnection npgsqlConn)
            throw new InvalidOperationException("Провайдер должен быть Npgsql PostgreSQL.");

        await conn.OpenAsync(cancellationToken);
        try
        {
            // Делаем пачками по batchSize строк
            for (int i = 0; i < list.Count; i += batchSize)
            {
                var chunk = list.Skip(i).Take(batchSize).ToList();
                // Строим команду INSERT ... VALUES (...),(...),... ON CONFLICT DO NOTHING
                var cmd = npgsqlConn.CreateCommand();

                var sbValues = new StringBuilder();
                var parameters = new List<NpgsqlParameter>();
                int paramIndex = 0;

                foreach (var entity in chunk)
                {
                    sbValues.Append("(");
                    for (int j = 0; j < props.Count; j++)
                    {
                        var prop = props[j];
                        // Получаем значение свойства через рефлексию
                        var val = prop.PropertyInfo?.GetValue(entity);
                        if (prop.ClrType.IsEnum && val != null)
                        {
                            val = EnumConverters.ToEnumString((Enum)val);
                        }
                        // Параметр без @ в названии; в SQL используем @pX
                        string paramName = "p" + paramIndex;
                        sbValues.Append($"@{paramName}");
                        // Добавляем параметр (Npgsql сам подберёт тип из значения)
                        var param = new NpgsqlParameter(paramName, val ?? DBNull.Value);
                        parameters.Add(param);

                        if (j < props.Count - 1) sbValues.Append(", ");
                        paramIndex++;
                    }
                    sbValues.Append(")");
                    if (entity != chunk.Last()) sbValues.Append(", ");
                }

                // Финальная SQL-команда
                cmd.CommandText = 
                    $"INSERT INTO {tableFullName} ({columnList}) VALUES {sbValues} ON CONFLICT DO NOTHING;";
                // Добавляем все параметры к команде
                cmd.Parameters.AddRange(parameters.ToArray());

                // Выполняем
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        finally
        {
            await conn.CloseAsync();
        }

        return;

        // Подготовка имен (экранирование двойными кавычками)
        string Quote(string? name) => $"\"{name.Replace("\"", "\"\"")}\"";
    }
}