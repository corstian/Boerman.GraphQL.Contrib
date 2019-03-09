using GraphQL.DataLoader;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Boerman.GraphQL.Contrib.DataLoaders
{
    public static class GenericDataLoader
    {
        public static Expression<Func<T, bool>> MatchOn<T, TValue>(
            this ICollection<TValue> items,
            Expression<Func<T, TValue>> predicate)
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.Call(
                    Expression.Constant(items),
                    typeof(ICollection<TValue>).GetMethod("Contains", new[] { typeof(TValue) }),
                    predicate.Body
                ),
                predicate.Parameters
            );
        }

        /// <summary>
        /// Register a dataloader for T by the predicate provided.
        /// </summary>
        /// <typeparam name="T">The type to retrieve from the DbSet</typeparam>
        /// <typeparam name="TValue">The value to filter on</typeparam>
        /// <param name="dataLoader">A dataloader to use</param>
        /// <param name="dbSet">Entity Framework DbSet</param>
        /// <param name="predicate">The predicate to select a key to filter on</param>
        /// <param name="value">Value to filter items on</param>
        /// <returns>T as specified by the predicate and TValue</returns>
        public static async Task<T> EntityLoader<T, TValue>(
            this IDataLoaderContextAccessor dataLoader,
            DbSet<T> dbSet,
            Expression<Func<T, TValue>> predicate,
            TValue value)
            where T : class
        {
            if (value == null) return default;

            var loader = dataLoader.Context.GetOrAddBatchLoader<TValue, T>(
                $"{typeof(T).Name}-{predicate.ToString()}",
                async (items) =>
                {
                    return await dbSet
                        .AsNoTracking()
                        .Where(items
                            .ToList()
                            .MatchOn(predicate))
                        .ToDictionaryAsync(predicate.Compile());
                });

            var task = loader.LoadAsync(value);
            return await task;
        }

        /// <summary>
        /// Register a dataloader for an IEnumerable<T> by the predicate provided.
        /// </summary>
        /// <typeparam name="T">The type to retrieve from the DbSet</typeparam>
        /// <typeparam name="TValue">The value to filter on</typeparam>
        /// <param name="dataLoader">A dataloader to use</param>
        /// <param name="dbSet">Entity Framework DbSet</param>
        /// <param name="predicate">The predicate to select a key to filter on</param>
        /// <param name="value">Value to filter items on</param>
        /// <returns>IEnumerable<T> as specified by the predicate and TValue</returns>
        public static async Task<IEnumerable<T>> EntityCollectionLoader<T, TValue>(
            this IDataLoaderContextAccessor dataLoader,
            DbSet<T> dbSet,
            Expression<Func<T, TValue>> predicate,
            TValue value)
            where T : class
        {
            if (value == null) return default;

            var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<TValue, T>(
                $"{typeof(T).Name}-{predicate.ToString()}",
                (items) =>
                {
                    var compiledPredicate = predicate.Compile();

                    return Task.FromResult(dbSet
                        .AsNoTracking()
                        .Where(items
                            .ToList()
                            .MatchOn(predicate))
                        .ToLookup(compiledPredicate));
                });

            var task = loader.LoadAsync(value);
            return await task;
        }
    }
}
