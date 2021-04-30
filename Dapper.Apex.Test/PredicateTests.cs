using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper.Apex;
using Dapper.Apex.Test.Database;
using Dapper.Apex.Test.Models;
using MySqlConnector;
using Xunit;

namespace Dapper.Apex.Test
{
    public class PredicateTests
    {
        [Theory(DisplayName = "Get Where Clause")]
        [ClassData(typeof(DbConnectionGenerator))]
        public void TestPredicate(IDbConnection connection)
        {
            var where =
                Where<ModelX>
                    .Add((m) => m.Prop1, CompareOperator.Equal, "Test")
                    .AndGroup()
                        .AddGroup()
                            .Add(m => m.Prop1, CompareOperator.Equal, "XXX")
                            .And(m => m.Prop4, CompareOperator.LowerOrEqual, 5)
                            .OrGroup()
                                .Add(m => m.Prop4, CompareOperator.GreaterThan, 0)
                                .Or(m => m.Prop4, CompareOperator.LowerThan, 5)
                                .EndGroup()
                            .EndGroup()
                        .And(m => m.Prop2, CompareOperator.NotEqual, "ABC")
                        .Or(m => m.Prop4, CompareOperator.GreatOrEqual, 10)
                        .EndGroup()
                    .Build(connection);

            bool exist = connection.Exists<Model>(key);
            connection.Upsert()

            Assert.NotNull(where);

            if (connection is SqlConnection)
            {
                Assert.Equal("[Prop1] = @Prop1_0 AND (([Prop1] = @Prop1_1 AND [Prop4] <= @Prop4_2 OR ([Prop4] > @Prop4_3 OR [Prop4] < @Prop4_4)) AND [Prop2] <> @Prop2_5 OR [Prop4] >= @Prop4_6)", where.Sql);
            }
            else if (connection is MySqlConnection)
            {
                Assert.Equal("`Prop1` = @Prop1_0 AND ((`Prop1` = @Prop1_1 AND `Prop4` <= @Prop4_2 OR (`Prop4` > @Prop4_3 OR `Prop4` < @Prop4_4)) AND `Prop2` <> @Prop2_5 OR `Prop4` >= @Prop4_6)", where.Sql);
            }

            Assert.Contains("Prop1_0", where.Params.ParameterNames);
            Assert.Contains("Prop1_1", where.Params.ParameterNames);
            Assert.Contains("Prop4_2", where.Params.ParameterNames);
            Assert.Contains("Prop4_3", where.Params.ParameterNames);
            Assert.Contains("Prop4_4", where.Params.ParameterNames);
            Assert.Contains("Prop2_5", where.Params.ParameterNames);
            Assert.Contains("Prop4_6", where.Params.ParameterNames);

            Assert.Equal("Test", where.Params.Get<string>("@Prop1_0"));
            Assert.Equal("XXX", where.Params.Get<string>("@Prop1_1"));
            Assert.Equal(5, where.Params.Get<int>("@Prop4_2"));
            Assert.Equal(0, where.Params.Get<int>("@Prop4_3"));
            Assert.Equal(5, where.Params.Get<int>("@Prop4_4"));
            Assert.Equal("ABC", where.Params.Get<string>("@Prop2_5"));
            Assert.Equal(10, where.Params.Get<int>("@Prop4_6"));
        }
    }
}
