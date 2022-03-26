using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Apex.Test.Database
{
    public class DbConnectionGenerator : IEnumerable<object[]>
    {

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { CreateSqlServerConnection(true) };
            yield return new object[] { CreateMySqlConnection(true) };
        }

        public static SqlConnection CreateSqlServerConnection(bool addDatabase)
        {
            return new System.Data.SqlClient.SqlConnection($"Server=localhost;Integrated Security=true;{(addDatabase ? "Database=DapperTest;" : string.Empty)}");
        }

        public static MySqlConnection CreateMySqlConnection(bool addDatabase)
        {
            return new MySqlConnector.MySqlConnection($"server=127.0.0.1;uid=admin;pwd=123;{(addDatabase ? "database=DapperTest;" : string.Empty)}");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
