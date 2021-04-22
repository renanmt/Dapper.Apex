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
    public static partial class DapperApex
    {
        /// <summary>
        /// Retrieves an entity from the database by its key.
        /// </summary>
        /// <typeparam name="T">The type of the entity to be retrieved.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="key">The value, object or Tuple representing the entity key.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>The entity object retrieved from the database.</returns>
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

        /// <summary>
        /// Retrieves all entities of a given type from the database.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be retrieved.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>A collection of entity objects retrieved from the database.</returns>
        public static async Task<IEnumerable<T>> GetAllAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var typeInfo = TypeHelper.GetTypeInfo(typeof(T));
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var objects = await connection.QueryAsync<T>(queryInfo.SelectAllQuery, transaction: transaction, commandTimeout: commandTimeout);

            return objects;
        }

        /// <summary>
        /// Retrieves the total count of entities of a given type in the database.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be retrieved.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>The total count of entities.</returns>
        public static async Task<long> GetCountAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var typeInfo = TypeHelper.GetTypeInfo(typeof(T));
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = await connection.ExecuteScalarAsync<long>(queryInfo.SelectCountQuery, transaction: transaction, commandTimeout: commandTimeout);

            return count;
        }
    }
}
