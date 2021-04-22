﻿using Dapper.Apex.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Apex
{
    public static partial class DapperApex
    {
        public static bool Update<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToUpdate == null)
                throw new ArgumentNullException(nameof(entityToUpdate));

            var type = typeof(T);
            
            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.UpdateQuery, entityToUpdate, transaction, commandTimeout);
            return count > 0;
        }

        public static bool UpdateMany<T>(this IDbConnection connection, IEnumerable<T> entitiesToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entitiesToUpdate == null)
                throw new ArgumentNullException(nameof(entitiesToUpdate));

            if (!entitiesToUpdate.Any()) return false;

            var type = typeof(T);

            TypeHelper.IsCollection(ref type);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.UpdateQuery, entitiesToUpdate, transaction, commandTimeout);
            return count > 0;
        }

        public static bool UpdateFields<T>(this IDbConnection connection, T entityToUpdate, IEnumerable<string> fieldsToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToUpdate == null)
                throw new ArgumentNullException(nameof(entityToUpdate));

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var sql = QueryHelper.GetUpdateFieldsQuery(connection, typeInfo, queryInfo, fieldsToUpdate);

            var count = connection.Execute(sql, entityToUpdate, transaction, commandTimeout);
            return count > 0;
        }

        public static bool UpdateExcept<T>(this IDbConnection connection, T entityToUpdate, IEnumerable<string> fieldsToIgnore, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToUpdate == null)
                throw new ArgumentNullException(nameof(entityToUpdate));

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var sql = QueryHelper.GetUpdateFieldsQuery(connection, typeInfo, queryInfo, fieldsToIgnore, true);

            var count = connection.Execute(sql, entityToUpdate, transaction, commandTimeout);
            return count > 0;
        }
    }
}
