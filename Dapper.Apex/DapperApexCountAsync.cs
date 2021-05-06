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
        /// Retrieves the total count of entities of a given type in the database.
        /// </summary>
        /// <typeparam name="T">The type of the entities to be retrieved.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="transaction">The database transaction to be used in the operation.</param>
        /// <param name="commandTimeout">The operation timeout in milliseconds.</param>
        /// <returns>The total count of entities.</returns>
        public static async Task<long> CountAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var typeInfo = TypeHelper.GetTypeInfo(typeof(T));
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var count = await connection.ExecuteScalarAsync<long>(queryInfo.SelectCountQuery, transaction: transaction, commandTimeout: commandTimeout);

            return count;
        }
    }
}
