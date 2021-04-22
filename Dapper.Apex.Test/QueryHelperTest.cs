using Dapper.Apex.Query;
using Dapper.Apex.Test.Database;
using Dapper.Apex.Test.Models;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace Dapper.Apex.Test
{
    public class QueryHelperTest
    {
        [Fact(DisplayName = "Flush Cache")]
        public void FlushCache()
        {
            var connection = new SqlConnection();

            var typeInfo = TypeHelper.GetTypeInfo(typeof(ModelX));
            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            QueryHelper.UpdateFieldsQueryCache.TryAdd("test", "test");

            Assert.NotNull(queryInfo);
            Assert.NotEmpty(QueryHelper.QueryInfos);
            Assert.NotEmpty(QueryHelper.UpdateFieldsQueryCache);

            QueryHelper.FlushCache();

            Assert.Empty(QueryHelper.QueryInfos);
            Assert.Empty(QueryHelper.UpdateFieldsQueryCache);
        }

        [Theory(DisplayName = "Get Query Info")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void GetQueryInfo(IDbConnection connection)
        {
            var typeInfo = TypeHelper.GetTypeInfo(typeof(ModelX));

            QueryHelper.FlushCache();

            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            Assert.NotNull(queryInfo);
            Assert.NotEmpty(queryInfo.SelectQuery);
            Assert.NotEmpty(queryInfo.SelectAllQuery);
            Assert.NotEmpty(queryInfo.InsertQuery);
            Assert.NotEmpty(queryInfo.UpdateQuery);
            Assert.NotEmpty(queryInfo.DeleteQuery);

            if (connection is SqlConnection)
            {
                Assert.Equal("select [Id], [Prop1], [Prop2], [Prop3] from [Model1] where [Id] = @Id", queryInfo.SelectQuery);
                Assert.Equal("select [Id], [Prop1], [Prop2], [Prop3] from [Model1]", queryInfo.SelectAllQuery);
                Assert.Equal("insert into [Model1] ([Prop1], [Prop3]) values (@Prop1, @Prop3)", queryInfo.InsertQuery);
                Assert.Equal("insert into [Model1] ([Prop1], [Prop3]) values (PARAMS)", queryInfo.InsertNoValuesQuery);
                Assert.Equal("update [Model1] set [Prop1] = @Prop1, [Prop3] = @Prop3 where [Id] = @Id", queryInfo.UpdateQuery);
                Assert.Equal("update [Model1] set FIELDS where [Id] = @Id", queryInfo.UpdateFieldsQuery);
                Assert.Equal("delete from [Model1] where [Id] = @Id", queryInfo.DeleteQuery);
                Assert.Equal("delete from [Model1]", queryInfo.DeleteAllQuery);
            }
            else if (connection is MySqlConnection)
            {
                Assert.Equal("select `Id`, `Prop1`, `Prop2`, `Prop3` from `Model1` where `Id` = @Id", queryInfo.SelectQuery);
                Assert.Equal("select `Id`, `Prop1`, `Prop2`, `Prop3` from `Model1`", queryInfo.SelectAllQuery);
                Assert.Equal("insert into `Model1` (`Prop1`, `Prop3`) values (@Prop1, @Prop3)", queryInfo.InsertQuery);
                Assert.Equal("insert into `Model1` (`Prop1`, `Prop3`) values (PARAMS)", queryInfo.InsertNoValuesQuery);
                Assert.Equal("update `Model1` set `Prop1` = @Prop1, `Prop3` = @Prop3 where `Id` = @Id", queryInfo.UpdateQuery);
                Assert.Equal("update `Model1` set FIELDS where `Id` = @Id", queryInfo.UpdateFieldsQuery);
                Assert.Equal("delete from `Model1` where `Id` = @Id", queryInfo.DeleteQuery);
                Assert.Equal("delete from `Model1`", queryInfo.DeleteAllQuery);
            }

            var queryInfo2 = QueryHelper.GetQueryInfo(connection, typeInfo);

            Assert.Same(queryInfo, queryInfo2);
        }

        [Theory(DisplayName = "Get Surrogate Key Return Query")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void GetSurrogateKeyReturnQuery(IDbConnection connection)
        {
            var surrogateKeyReturnQuery = QueryHelper.GetSurrogateKeyReturnQuery(connection);

            Assert.NotEmpty(surrogateKeyReturnQuery);

            if (connection is SqlConnection)
            {
                var sqlServerDbHelper = new SqlServerDbHelper();
                Assert.Equal(sqlServerDbHelper.GetSurrogateKeyReturnQuery(), surrogateKeyReturnQuery);
            }
            else if (connection is MySqlConnection)
            {
                var mysqlDbHelper = new MySqlDbHelper();
                Assert.Equal(mysqlDbHelper.GetSurrogateKeyReturnQuery(), surrogateKeyReturnQuery);
            }
        }

        [Theory(DisplayName = "Get Query for Insert Many")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void GetInsertManyQuery(IDbConnection connection)
        {
            var typeInfo = TypeHelper.GetTypeInfo(typeof(ModelX));

            QueryHelper.FlushCache();

            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var entities = new List<ModelX>()
            {
                {  new ModelX() },
                {  new ModelX() },
            };

            var sql = QueryHelper.GetInsertManyQuery(connection, typeInfo, queryInfo, entities.Count);

            if (connection is SqlConnection)
            {
                var sqlExpected =
                    "insert into [Model1] ([Prop1], [Prop3]) values (@Prop1_0, @Prop3_0);select SCOPE_IDENTITY() id;\r\n" +
                    "insert into [Model1] ([Prop1], [Prop3]) values (@Prop1_1, @Prop3_1);select SCOPE_IDENTITY() id;\r\n";

                Assert.Equal(sqlExpected, sql);
            }
            else if (connection is MySqlConnection)
            {
                var sqlExpected =
                    "insert into `Model1` (`Prop1`, `Prop3`) values (@Prop1_0, @Prop3_0);select LAST_INSERT_ID() id;\r\n" +
                    "insert into `Model1` (`Prop1`, `Prop3`) values (@Prop1_1, @Prop3_1);select LAST_INSERT_ID() id;\r\n";

                Assert.Equal(sqlExpected, sql);
            }
        }

        [Theory(DisplayName = "Get Update Fields query Inclusive")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void GetUpdateFieldsQueryInclusive(IDbConnection connection)
        {
            var typeInfo = TypeHelper.GetTypeInfo(typeof(ModelX));

            QueryHelper.FlushCache();

            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var fields = new List<string> { { "Prop3" } };

            var sql = QueryHelper.GetUpdateFieldsQuery(connection, typeInfo, queryInfo, fields);

            if (connection is SqlConnection)
            {
                var sqlExpected = "update [Model1] set [Prop3] = @Prop3 where [Id] = @Id";

                Assert.Equal(sqlExpected, sql);
            }
            else if (connection is MySqlConnection)
            {
                var sqlExpected = "update `Model1` set `Prop3` = @Prop3 where `Id` = @Id";

                Assert.Equal(sqlExpected, sql);
            }
        }

        [Theory(DisplayName = "Get Update Fields query Exclusive")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void GetUpdateFieldsQueryExclusive(IDbConnection connection)
        {
            var typeInfo = TypeHelper.GetTypeInfo(typeof(ModelX));

            QueryHelper.FlushCache();

            var queryInfo = QueryHelper.GetQueryInfo(connection, typeInfo);

            var fields = new List<string> { { "Prop3" } };

            var sql = QueryHelper.GetUpdateFieldsQuery(connection, typeInfo, queryInfo, fields, true);

            if (connection is SqlConnection)
            {
                var sqlExpected = "update [Model1] set [Prop1] = @Prop1 where [Id] = @Id";

                Assert.Equal(sqlExpected, sql);
            }
            else if (connection is MySqlConnection)
            {
                var sqlExpected = "update `Model1` set `Prop1` = @Prop1 where `Id` = @Id";

                Assert.Equal(sqlExpected, sql);
            }
        }
    }
}
