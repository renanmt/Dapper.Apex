using Dapper.Apex.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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
        public static async Task InsertAsync<T>(this IDbConnection connection, T entity, 
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            ValidateEntityForInsert(entity);

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            if (typeInfo.KeyType == KeyType.Surrogate)
            {
                var sql = $"{queryInfo.InsertQuery};{QueryHelper.GetSurrogateKeyReturnQuery(connection)};";
                var res = (await connection.QueryMultipleAsync(sql, entity, transaction, commandTimeout)).Read();

                var id = res.First().id;

                var keyProperty = typeInfo.PrimaryKeyProperties.First();
                keyProperty.SetValue(entity, Convert.ChangeType(id, keyProperty.PropertyType), null);
            }
            else
            {
                await connection.ExecuteAsync(queryInfo.InsertQuery, entity, transaction, commandTimeout);
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
        public static async Task<long> InsertManyAsync<T>(this IDbConnection connection, IEnumerable<T> entitiesToInsert,
            IDbTransaction transaction = null, int? commandTimeout = null,
            OperationMode insertMode = OperationMode.OneByOne) where T : class
        {
            if (entitiesToInsert == null)
                throw new ArgumentNullException(nameof(entitiesToInsert));

            if (!entitiesToInsert.Any()) return 0;

            var type = typeof(T);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            if (insertMode == OperationMode.SingleShot)
                return await InsertManySingleShotAsync(connection, entitiesToInsert, typeInfo, queryInfo, transaction, commandTimeout);

            return await InsertManyOneByOneAsync(connection, entitiesToInsert, typeInfo, queryInfo, transaction, commandTimeout);
        }

        private static async Task<long> InsertManySingleShotAsync<T>(this IDbConnection connection, IEnumerable<T> entitiesToInsert,
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
                var dbRes = await connection.QueryMultipleAsync(sql, dynParams, transaction, commandTimeout);

                var keyProperty = typeInfo.PrimaryKeyProperties.First();

                foreach (var entity in entitiesToInsert)
                {
                    var res = dbRes.Read();
                    var id = res.First().id;
                    keyProperty.SetValue(entity, Convert.ChangeType(id, keyProperty.PropertyType), null);
                }

                return entitiesToInsert.Count();
            }
            else
            {
                await connection.ExecuteAsync(sql, dynParams, transaction, commandTimeout);
                return entitiesToInsert.Count();
            }
        }

        private static async Task<long> InsertManyOneByOneAsync<T>(this IDbConnection connection, IEnumerable<T> entitiesToInsert,
            TypeInfo typeInfo, TypeQueryInfo queryInfo,
            IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var sql = QueryHelper.GetInsertQuery(connection, typeInfo, queryInfo);

            if (typeInfo.KeyType == KeyType.Surrogate)
            {
                foreach (var entity in entitiesToInsert)
                {
                    var dbRes = await connection.QueryMultipleAsync(sql, entity, transaction, commandTimeout);

                    var keyProperty = typeInfo.PrimaryKeyProperties.First();

                    var res = dbRes.Read();
                    var id = res.First().id;
                    keyProperty.SetValue(entity, Convert.ChangeType(id, keyProperty.PropertyType), null);
                }

                return entitiesToInsert.Count();
            }
            else
            {
                await connection.ExecuteAsync(sql, entitiesToInsert, transaction, commandTimeout);
                return entitiesToInsert.Count();
            }
        }
    }
}
