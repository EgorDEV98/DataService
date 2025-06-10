using System.Linq.Expressions;

namespace DataService.Data.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Выполняет фильтрацию по условию
    /// </summary>
    /// <param name="source"></param>
    /// <param name="condition">Условие при котором фильтрация будет выполняться</param>
    /// <param name="predicate">Фильтрация</param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IQueryable<TSource> WhereIf<TSource>(
        this IQueryable<TSource> source,  bool condition, Expression<Func<TSource, bool>> predicate)
        => condition ? source.Where(predicate) : source;
    
    /// <summary>
    /// Выполняет фильтрацию по условию
    /// </summary>
    /// <param name="source"></param>
    /// <param name="condition">Условие</param>
    /// <param name="ifPredicate">Выполняется, если condition = true</param>
    /// <param name="elsePredicate">Выполняется, если condition = false</param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IQueryable<TSource> WhereIfElse<TSource>(
        this IQueryable<TSource> source,  
        bool condition, Expression<Func<TSource, bool>> ifPredicate, Expression<Func<TSource, bool>> elsePredicate)
        => condition ? source.Where(ifPredicate) : source.Where(elsePredicate);
}