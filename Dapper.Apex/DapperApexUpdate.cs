using Dapper.Apex.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Apex
{
    public static partial class DapperApex
    {
        /// <summary>
        /// Updates a given entity in the database.
        /// </summary>
        /// <typeparam name="T">The type of the entity to be updated.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entity">The entity object to be updated.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if the entity was found and successfully updated.</returns>
        public static bool Update<T>(this IDbConnection connection, T entity, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            ValidateEntityForUpdate(entity);

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.UpdateQuery, entity, transaction, commandTimeout);
            return count > 0;
        }

        /// <summary>
        /// Updates all entities of a given collection in the database.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be updated.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entities">The collection of entity objects to be updated.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if all entities were found and successfully updated.</returns>
        public static bool UpdateMany<T>(this IDbConnection connection, IEnumerable<T> entities, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (!entities.Any()) return false;

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = connection.Execute(queryInfo.UpdateQuery, entities, transaction, commandTimeout);
            return count == entities.Count();
        }

        /// <summary>
        /// Updates specific fields of a given entity in the database.
        /// </summary>
        /// <typeparam name="T">The type of the entity to be updated.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entity">The entity object to be updated.</param>
        /// <param name="fieldsToUpdate">The list of fields to be updated.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if the entity was found and successfully updated.</returns>
        public static bool UpdateFields<T>(this IDbConnection connection, T entity, 
            IEnumerable<string> fieldsToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            ValidateEntityForUpdate(entity);

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var sql = QueryHelper.GetUpdateFieldsQuery(connection, typeInfo, queryInfo, fieldsToUpdate);

            var count = connection.Execute(sql, entity, transaction, commandTimeout);
            return count > 0;
        }

        /// <summary>
        /// Updates all fields of a given entity in the database, except for a list of fields to be ignored.
        /// </summary>
        /// <typeparam name="T">The type of the entity to be updated.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entity">The entity object to be updated.</param>
        /// <param name="fieldsToIgnore">The list of fields to be ignored in the update.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>True if the entity was found and successfully updated.</returns>
        public static bool UpdateExcept<T>(this IDbConnection connection, T entity, 
            IEnumerable<string> fieldsToIgnore, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            ValidateEntityForUpdate(entity);

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var sql = QueryHelper.GetUpdateFieldsQuery(connection, typeInfo, queryInfo, fieldsToIgnore, true);

            var count = connection.Execute(sql, entity, transaction, commandTimeout);
            return count > 0;
        }

        private static void ValidateEntityForUpdate<T>(T entity) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity is IEnumerable)
                throw new NotSupportedException("Method not supported for collections. Try UpdateMany instead.");
        }
    }
}
