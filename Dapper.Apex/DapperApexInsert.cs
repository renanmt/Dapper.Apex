using Dapper.Apex.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace Dapper.Apex
{
    public static partial class DapperApex
    {
        /// <summary>
        /// Inserts a given entity to its related database table.
        /// </summary>
        /// <remarks>If the given entity has a surrogate key, the object will be updated with the new key.</remarks>
        /// <typeparam name="T">The type of the entity to be inserted.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entity">The entity object to be inserted.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        public static void Insert<T>(this IDbConnection connection, T entity,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            ValidateEntityForInsert(entity);

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var sql = QueryHelper.GetInsertQuery(connection, typeInfo, queryInfo);

            if (typeInfo.KeyType == KeyType.Surrogate)
            {
                using (var reader = connection.QueryMultiple(sql, entity, transaction, commandTimeout))
                {
                    var res = reader.Read();
                    var id = res.First().id;

                    var keyProperty = typeInfo.PrimaryKeyProperties.First();
                    keyProperty.SetValue(entity, Convert.ChangeType(id, keyProperty.PropertyType), null);
                }
            }
            else
            {
                connection.Execute(sql, entity, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// Inserts all entities of a given collection to their related table in the database.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be inserted.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entities">The collection of entity objects to be inserted.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <param name="operationMode">The operation execution mode. Choose it accordingly to your environments latency and number of entities.</param>
        /// <returns>The number of rows affected by the operation.</returns>
        public static long InsertMany<T>(this IDbConnection connection, IEnumerable<T> entities,
            IDbTransaction transaction = null, int? commandTimeout = null,
            OperationMode operationMode = OperationMode.OneByOne) where T : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (!entities.Any()) return 0;

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            if (operationMode == OperationMode.SingleShot)
                return InsertManySingleShot(connection, entities, typeInfo, queryInfo, transaction, commandTimeout);

            return InsertManyOneByOne(connection, entities, typeInfo, queryInfo, transaction, commandTimeout);
        }

        private static long InsertManySingleShot<T>(this IDbConnection connection, IEnumerable<T> entitiesToInsert,
            TypeInfo typeInfo, TypeQueryInfo queryInfo,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var sql = QueryHelper.GetInsertManyQuery(connection, typeInfo, queryInfo, entitiesToInsert.Count());

            DynamicParameters dynParams = new DynamicParameters();

            for (int i = 0; i < entitiesToInsert.Count(); i++)
            {
                foreach (var prop in typeInfo.InsertableProperties)
                {
                    dynParams.Add(QueryHelper.GetParamName(prop, $"_{i}"), prop.GetValue(entitiesToInsert.ElementAt(i)));
                }
            }

            if (typeInfo.KeyType == KeyType.Surrogate)
            {
                using (var reader = connection.QueryMultiple(sql, dynParams, transaction, commandTimeout))
                {
                    var keyProperty = typeInfo.PrimaryKeyProperties.First();
                    
                    foreach (var entity in entitiesToInsert)
                    {
                        var res = reader.Read();
                        var id = res.First().id;
                        keyProperty.SetValue(entity, Convert.ChangeType(id, keyProperty.PropertyType), null);
                    }

                    return entitiesToInsert.Count();
                }
            }
            else
            {
                connection.Execute(sql, dynParams, transaction, commandTimeout);
                return entitiesToInsert.Count();
            }
        }

        private static long InsertManyOneByOne<T>(this IDbConnection connection, IEnumerable<T> entitiesToInsert,
            TypeInfo typeInfo, TypeQueryInfo queryInfo,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var sql = QueryHelper.GetInsertQuery(connection, typeInfo, queryInfo);

            if (typeInfo.KeyType == KeyType.Surrogate)
            {
                foreach (var entity in entitiesToInsert)
                {
                    using (var reader = connection.QueryMultiple(sql, entity, transaction, commandTimeout))
                    {
                        var keyProperty = typeInfo.PrimaryKeyProperties.First();

                        var res = reader.Read();
                        var id = res.First().id;

                        keyProperty.SetValue(entity, Convert.ChangeType(id, keyProperty.PropertyType), null);
                    }
                }

                return entitiesToInsert.Count();
            }
            else
            {
                connection.Execute(sql, entitiesToInsert, transaction, commandTimeout);
                return entitiesToInsert.Count();
            }
        }

        private static void ValidateEntityForInsert<T>(T entity) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity is IEnumerable)
                throw new NotSupportedException("Method not supported for collections. Try InsertMany instead.");
        }
    }
}
