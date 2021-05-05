using Dapper.Apex.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

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
        public static bool Delete<T>(this IDbConnection connection, object key,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            DynamicParameters dynParams = GetParameters(type, key, typeInfo.PrimaryKeyProperties);

            var count = connection.Execute(queryInfo.DeleteQuery, dynParams, transaction, commandTimeout);
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
        public static bool Delete<T>(this IDbConnection connection, T entity,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            ValidateEntityForDelete(entity);

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.DeleteQuery, entity, transaction, commandTimeout);
            return count > 0;
        }

        /// <summary>
        /// Deletes all entities of a given collection from the database.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be deleted.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entities">The collection of entity objects to be deleted.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if all entities were found and successfully deleted.</returns>
        public static bool DeleteMany<T>(this IDbConnection connection, IEnumerable<T> entities,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (!entities.Any()) return false;

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.DeleteQuery, entities, transaction, commandTimeout);
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
        public static long DeleteAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.DeleteAllQuery, null, transaction, commandTimeout);
            return count;
        }

        private static void ValidateEntityForDelete<T>(T entity) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity is IEnumerable)
                throw new NotSupportedException("Method not supported for collections. Try DeleteMany instead.");
        }
    }
}
