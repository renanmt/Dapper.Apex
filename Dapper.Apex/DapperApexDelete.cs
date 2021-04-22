using Dapper;
using Dapper.Apex.Query;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Dapper.Apex
{
    /// <summary>
    /// Extension methods for Dapper
    /// </summary>
    public static partial class DapperApex
    {
        public static bool Delete<T>(this IDbConnection connection, ITuple keys, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            DynamicParameters dynParams = GenerateGetParams(type, keys, typeInfo.PrimaryKeyProperties);

            var count = connection.Execute(queryInfo.DeleteQuery, dynParams, transaction, commandTimeout);
            return count > 0;
        }

        public static bool Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToDelete == null)
                throw new ArgumentNullException(nameof(entityToDelete));

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.DeleteQuery, entityToDelete, transaction, commandTimeout);
            return count > 0;
        }

        public static bool DeleteMany<T>(this IDbConnection connection, IEnumerable<T> entitiesToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entitiesToDelete == null)
                throw new ArgumentNullException(nameof(entitiesToDelete));

            if (!entitiesToDelete.Any()) return false;

            var type = typeof(T);

            TypeHelper.IsCollection(ref type);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.DeleteQuery, entitiesToDelete, transaction, commandTimeout);
            return count > 0;
        }

        public static long DeleteAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.DeleteAllQuery, null, transaction, commandTimeout);
            return count;
        }
    }
}
