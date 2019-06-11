using Dapper;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types.Relay.DataObjects;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Boerman.GraphQL.Contrib
{
    public static class QueryExtensions
    {
        [Pure]
        public static Query Slice(
            this Query query,
            string after = null,
            int first = 0,
            string before = null,
            int last = 0,
            string column = "Id")
        {
            var queryClone = query.Clone();

            var order = new SqlServerCompiler()
                .CompileOrders(new SqlResult
                {
                    Query = queryClone
                });

            queryClone.Clauses.RemoveAll(q => q.Component == "order");

            if (string.IsNullOrWhiteSpace(order))
                throw new Exception($"{nameof(query)} does not have an order by clause");

            queryClone.SelectRaw($"ROW_NUMBER() OVER({order}) AS [RowNumber]");

            var internalQuery = new Query()
                .With("q", queryClone)
                .Select("*")
                .SelectRaw("(SELECT COUNT(*) FROM [q]) AS [RowCount]")
                .From("q");

            // Select all rows after provided cursor
            if (!String.IsNullOrWhiteSpace(after))
            {
                internalQuery.Where("RowNumber", ">",
                    new Query("q")
                        .Select("RowNumber")
                        .Where(column, after));
            }

            // Select all rows before provided cursor
            if (!String.IsNullOrWhiteSpace(before))
            {
                internalQuery.Where("RowNumber", "<",
                    new Query("q")
                        .Select("RowNumber")
                        .Where(column, before));
            }

            // Select the first x amount of rows
            if (first > 0)
            {
                // If the after cursor is defined
                if (!String.IsNullOrWhiteSpace(after))
                {
                    internalQuery.Where("RowNumber", "<=",
                        new Query("q")
                            .SelectRaw($"[RowNumber] + {first}")
                            .Where(column, after));
                }
                // If no after cursor is defined
                else
                {
                    internalQuery.Where("RowNumber", "<=", first);
                }
            }

            // Select the last x amount of rows
            if (last > 0)
            {
                // If the before cursor is defined
                if (!String.IsNullOrWhiteSpace(before))
                {
                    internalQuery.Where("RowNumber", ">=",
                        new Query("q")
                            .SelectRaw($"[RowNumber] - {last}")
                            .Where(column, before));
                }
                // If we have to take data all the way from the back
                else
                {
                    internalQuery.Where("RowNumber", ">",
                        new Query("q")
                            .SelectRaw($"MAX([RowNumber]) - {last}"));
                }

                // ToDo: Implement the case where the `first` argument is provided and we want to take the last x from this selection, according to the GraphQL spec.
            }

            return internalQuery;
        }

        public static async Task<Connection<T>> ToConnection<T, TSource>(
            this Query query,
            ResolveConnectionContext<TSource> context,
            Func<T, string> cursorSelector,
            string cursorField = "Id")
            where T : class
            where TSource : class
        {
            var xQuery = query as XQuery;

            if (xQuery == null) throw new ArgumentException("Make sure the query object is instantiated from a queryFactory", nameof(query));

            //var countQuery = xQuery.Clone();

            //countQuery
            //    .Clauses
            //    .RemoveAll(q => q.Component == "select"
            //                 || q.Component == "order");

            //var totalCount = (await countQuery
            //    .SelectRaw("MAX(RowNumber)")
            //    .GetAsync<int>())
            //    .SingleOrDefault();

            //if (totalCount == 0) return new Connection<T>();

            if (!xQuery.Clauses.Any(q => q.Component == "select")) xQuery.Select("*");
            
            var statement = xQuery.Compiler.Compile(
                xQuery.Slice(
                    context.After?.FromCursor().ToString(),
                    context.First.GetValueOrDefault(0),
                    context.Before?.FromCursor().ToString(),
                    context.Last.GetValueOrDefault(0)));

            // Some custom mapping logic. Until the `RowNumber` field is found, all fields are considered
            // to be part of `T`. This methodology works because the last field selected is in fact the
            // `RowNumber` field (through the Slice function).
            //var dictionary = xQuery.Connection.Query(
            //    sql: statement.Sql,
            //    param: statement.NamedBindings,
            //    map: (T t, long i) => new KeyValuePair<long, T>(i, t),
            //    splitOn: "RowNumber")
            //    .ToDictionary(q => q.Key, q => q.Value);

            var totalCount = 0;

            var dictionary = xQuery.Connection.Query(
                sql: statement.Sql,
                param: statement.NamedBindings,
                map: (T t, (long rowNumber, long rowCount) meta) => {
                    totalCount = (int)meta.rowCount;
                    return new KeyValuePair<long, T>(meta.rowNumber, t);
                },
                splitOn: "RowNumber")
                .ToDictionary(q => q.Key, q => q.Value);

            if (!dictionary.Any()) return new Connection<T>();

            var connection = new Connection<T>
            {
                Edges = dictionary
                    .Select(q => new Edge<T>
                    {
                        Node = q.Value,
                        Cursor = cursorSelector.Invoke(q.Value)
                    })
                    .ToList(),
                TotalCount = totalCount,
                PageInfo = new PageInfo
                {
                    HasPreviousPage = dictionary.First().Key > 1,
                    HasNextPage = dictionary.Last().Key < totalCount
                }
            };

            connection.PageInfo.StartCursor = connection.Edges.First().Cursor;
            connection.PageInfo.EndCursor = connection.Edges.Last().Cursor;

            return connection;
        }

        public static async Task<Connection<T>> ToConnection<T, TSource>(
            this Query query,
            ResolveConnectionContext<TSource> context,
            string cursorField = "Id")
            where T : class, IId
            where TSource : class
        {
            return await query.ToConnection<T, TSource>(context, q => q.Id.ToCursor());
        }
        
        public static Query LeftJoinAs(this Query query, string table, string alias, string first, string second, string op = "=")
        {
            return query.LeftJoin(new Query(table).As(alias), q => q.On(first, second, op));
        }

        public static Query RightJoinAs(this Query query, string table, string alias, string first, string second, string op = "=")
        {
            return query.RightJoin(new Query(table).As(alias), q => q.On(first, second, op));
        }

        public static Query JoinAs(this Query query, string table, string alias, string first, string second, string op = "=")
        {
            return query.Join(new Query(table).As(alias), q => q.On(first, second, op));
        }
    }
}
