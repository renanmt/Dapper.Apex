using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Apex.Query
{
    public class SqlServerDbHelper : ISqlDbHelper
    {
        public string FormatDbEntityName(string columnName) => $"[{columnName}]";
        public string GetSurrogateKeyReturnQuery() => "select SCOPE_IDENTITY() id";
    }
}
