using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Apex.Query
{
    public class MySqlDbHelper : ISqlDbHelper
    {
        public string FormatDbEntityName(string columnName) => $"`{columnName}`";
        public string GetExistsQuery(string existsQueryTest) => $"select exists({existsQueryTest}) as `Exists`";
        public string GetSurrogateKeyReturnQuery() => "select LAST_INSERT_ID() id";
    }
}
