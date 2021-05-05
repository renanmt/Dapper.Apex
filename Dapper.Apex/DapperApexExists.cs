using Dapper.Apex.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Apex
{
    public static partial class DapperApex
    {
        /// <summary>
        /// Checks if an entity exists in database for given key.
        /// </summary>
        /// <typeparam name="T">The type of the entity to be checked existence.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="key">The tuple, value, collection, dictionary, expando object or object representing the entity key.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if the entity exists in the database.</returns>
        public static bool Exists<T>(this IDbConnection connection, object key, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var sql = QueryHelper.GetExistsQuery(connection, queryInfo);

            var dynParams = GetParameters(type, key, typeInfo.PrimaryKeyProperties);

            var exists = Convert.ToBoolean(connection.ExecuteScalar(sql, dynParams, transaction, commandTimeout));
            return exists;
        }

        /// <summary>
        /// Checks if an entity exists in database.
        /// </summary>
        /// <typeparam name="T">The type of the entity to be checked existence.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entity">The entity to be checked existence.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if the entity exists in the database.</returns>
        public static bool Exists<T>(this IDbConnection connection, T entity, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var sql = QueryHelper.GetExistsQuery(connection, queryInfo);

            var exists = Convert.ToBoolean(connection.ExecuteScalar(sql, entity, transaction, commandTimeout));
            return exists;
        }
    }
}
