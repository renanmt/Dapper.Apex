using Dapper;
using Dapper.Apex.Query;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dapper.Apex
{
    public static partial class DapperApex
    {
        /// <summary>
        /// Deletes an entity from the database by its key.
        /// </summary>
        /// <typeparam name="T">The type of the entity to be deleted.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="key">The tuple, value, collection, dictionary, expando object or object representing the entity key.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if the entity was found and successfully deleted.</returns>
        public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, object key, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            DynamicParameters dynParams = GetParameters(type, key, typeInfo.PrimaryKeyProperties);

            var count = await connection.ExecuteAsync(queryInfo.DeleteQuery, dynParams, transaction, commandTimeout);
            return count > 0;
        }

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <typeparam name="T">The type of the entity to be deleted.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entity">The entity object to be deleted.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if the entity was found and successfully deleted.</returns>
        public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            ValidateEntityForDelete(entity);

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = await connection.ExecuteAsync(queryInfo.DeleteQuery, entity, transaction, commandTimeout);
            return count > 0;
        }

        /// <summary>
        /// Deletes all entities of a given collection from the database.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be deleted.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entities">The collection of entities to be deleted.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if all entities were found and successfully deleted.</returns>
        public static async Task<bool> DeleteManyAsync<T>(this IDbConnection connection, IEnumerable<T> entities, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (!entities.Any()) return false;

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = await connection.ExecuteAsync(queryInfo.DeleteQuery, entities, transaction, commandTimeout);
            return count == entities.Count();
        }

        /// <summary>
        /// Deletes all entities of a given type from the database.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be deleted.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>The number of rows affected by the operation.</returns>
        public static async Task<long> DeleteAllAsync<T>(this IDbConnection connection, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = await connection.ExecuteAsync(queryInfo.DeleteAllQuery, null, transaction, commandTimeout);
            return count;
        }
    }
}
