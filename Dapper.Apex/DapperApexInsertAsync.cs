using Dapper.Apex.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Apex
{
    public static partial class DapperApex
    {
        public static async Task<long> InsertAsync<T>(this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToInsert == null)
                throw new ArgumentNullException(nameof(entityToInsert));

            var type = typeof(T);

            var isCollection = TypeHelper.IsCollection(ref type);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            if (!isCollection && typeInfo.KeyType == KeyType.Surrogate)
            {
                var sql = $"{queryInfo.InsertQuery};{QueryHelper.GetSurrogateKeyReturnQuery(connection)};";
                var res = (await connection.QueryMultipleAsync(sql, entityToInsert, transaction, commandTimeout)).Read();

                if (res.FirstOrDefault()?.id == null) return 0;
                var id = res.FirstOrDefault()?.id;

                var keyProperty = typeInfo.PrimaryKeyProperties.First();
                keyProperty.SetValue(entityToInsert, Convert.ChangeType(id, keyProperty.PropertyType), null);

                return 1;
            }
            else
            {
                var count = await connection.ExecuteAsync(queryInfo.InsertQuery, entityToInsert, transaction, commandTimeout);
                return count;
            }
        }

        public static async Task<long> InsertManyAsync<T>(this IDbConnection connection, IEnumerable<T> entitiesToInsert,
            IDbTransaction transaction = null, int? commandTimeout = null,
            InsertMode insertMode = InsertMode.OneByOne) where T : class
        {
            if (entitiesToInsert == null)
                throw new ArgumentNullException(nameof(entitiesToInsert));

            if (!entitiesToInsert.Any()) return 0;

            var type = typeof(T);

            TypeHelper.IsCollection(ref type);

            var typeInfo = TypeHelper.GetTypeInfo(type);
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            if (insertMode == InsertMode.SingleShot)
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
