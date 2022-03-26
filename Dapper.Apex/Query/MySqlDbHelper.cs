using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dapper.Apex.Query
{
    public class MySqlDbHelper : ISqlDbHelper
    {
        private readonly Regex _tableRegex = new Regex(@"^(?:\`?(?<schema>\w+)\`?\.)?(?:\`?(?<table>\w+)\`?)$");

        public string FormatColumnName(string columnName) => $"`{columnName}`";

        public string FormatTableName(string tableName)
        {
            var tn = _tableRegex.Match(tableName);
            var schema = tn.Groups["schema"].Success ? $"`{ tn.Groups["schema"].Value}`." : String.Empty;
            return $"{schema}`{tn.Groups["table"]}`";
        }

        public string GetExistsQuery(string existsQueryTest) => $"select exists({existsQueryTest}) as `Exists`";

        public string GetSurrogateKeyReturnQuery() => "select LAST_INSERT_ID() id";
    }
}
