using Dapper;
using Dapper.Apex.Query;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dapper.Apex
{
    /// <summary>
    /// Extension methods for Dapper
    /// </summary>
    public static partial class DapperApex
    {
        public static async Task<T> GetAsync<T>(this IDbConnection connection, dynamic key, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var type = typeof(T);
            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            DynamicParameters dynParams = GenerateGetParams(type, key, typeInfo.PrimaryKeyProperties);

            T obj = (
                await connection.QueryAsync<T>(queryInfo.SelectQuery, dynParams, transaction, commandTimeout: commandTimeout)
            )
            .FirstOrDefault();

            return obj;
        }

        public static async Task<IEnumerable<T>> GetAllAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var typeInfo = TypeHelper.GetTypeInfo(typeof(T));
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var objects = await connection.QueryAsync<T>(queryInfo.SelectAllQuery, transaction: transaction, commandTimeout: commandTimeout);

            return objects;
        }
    }
}
