using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Apex.Query
{
    public class SqlServerDbHelper : ISqlDbHelper
    {
        public string FormatDbEntityName(string columnName) => $"[{columnName}]";

        public string GetExistsQuery(string existsQueryTest) => $"select case when exists({existsQueryTest}) then 1 else 0 end as [Exists]";

        public string GetSurrogateKeyReturnQuery() => "select SCOPE_IDENTITY() id";
    }
}
