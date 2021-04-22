using Dapper;
using Dapper.Apex.Query;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dapper.Apex
{
    /// <summary>
    /// Extension methods for Dapper
    /// </summary>
    public static partial class DapperApex
    {
        public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, ITuple keys, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            DynamicParameters dynParams = GenerateGetParams(type, keys, typeInfo.PrimaryKeyProperties);

            var count = await connection.ExecuteAsync(queryInfo.DeleteQuery, dynParams, transaction, commandTimeout);
            return count > 0;
        }

        public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToDelete == null)
                throw new ArgumentNullException(nameof(entityToDelete));

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = await connection.ExecuteAsync(queryInfo.DeleteQuery, entityToDelete, transaction, commandTimeout);
            return count > 0;
        }

        public static async Task<bool> DeleteManyAsync<T>(this IDbConnection connection, IEnumerable<T> entitiesToDelete, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entitiesToDelete == null)
                throw new ArgumentNullException(nameof(entitiesToDelete));

            if (!entitiesToDelete.Any()) return false;

            var type = typeof(T);

            TypeHelper.IsCollection(ref type);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = await connection.ExecuteAsync(queryInfo.DeleteQuery, entitiesToDelete, transaction, commandTimeout);
            return count > 0;
        }

        public static async Task<long> DeleteAllAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = await connection.ExecuteAsync(queryInfo.DeleteAllQuery, null, transaction, commandTimeout);
            return count;
        }
    }
}
