using DataService.Integration.Models;

namespace DataService.Integration.Interfaces;

public interface IIndicatorBuilderProvider
{
    /// <summary>
    /// Запуск расчета индикаторов
    /// </summary>
    /// <param name="param">Параметры</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IndicatorResult> Calculate(CalculateIndicatorParams param, CancellationToken cancellationToken = default);
}