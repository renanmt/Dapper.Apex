using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dapper.Apex.Query
{
    public class SqlServerDbHelper : ISqlDbHelper
    {
        private readonly Regex _tableRegex = new Regex(@"^(?:\[?(?<schema>\w+)\]?\.)?(?:\[?(?<table>\w+)\]?)$");

        public string FormatColumnName(string columnName) => $"[{columnName}]";

        public string FormatTableName(string tableName)
        {
            var tn = _tableRegex.Match(tableName);
            var schema = tn.Groups["schema"].Success ? $"[{ tn.Groups["schema"].Value}]." : String.Empty;
            return $"{schema}[{tn.Groups["table"]}]";
        }

        public string GetExistsQuery(string existsQueryTest) => $"select case when exists({existsQueryTest}) then 1 else 0 end as [Exists]";

        public string GetSurrogateKeyReturnQuery() => "select SCOPE_IDENTITY() id";
    }
}
