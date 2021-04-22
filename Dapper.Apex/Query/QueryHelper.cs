using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dapper.Apex.Query
{
    public class TypeQueryInfo
    {
        public string SelectQuery { get; set; }
        public string SelectAllQuery { get; set; }
        public string InsertQuery { get; set; }
        public string InsertNoValuesQuery { get; set; }
        public string UpdateQuery { get; set; }
        public string UpdateFieldsQuery { get; set; }
        public string DeleteQuery { get; set; }
        public string DeleteAllQuery { get; set; }
    }

    public static class QueryHelper
    {
        private const string DefaultConnection = "sqlconnection";
        private const string ColumnEqualsToParamTemplate = "{0} = @{1}";
        private const string SelectTemplate = "select {0} from {1} where {2}";
        private const string SelectAllTemplate = "select {0} from {1}";
        private const string InsertTemplate = "insert into {0} ({1}) values ({2})";
        private const string InsertNoValuesTemplate = "insert into {0} ({1}) values (PARAMS)";
        private const string UpdateTemplate = "update {0} set {1} where {2}";
        private const string UpdateFieldsTemplate = "update {0} set FIELDS where {1}";
        private const string DeleteTemplate = "delete from {0} where {1}";
        private const string DeleteAllTemplate = "delete from {0}";

        public static readonly ConcurrentDictionary<RuntimeTypeHandle, TypeQueryInfo> QueryInfos = new ConcurrentDictionary<RuntimeTypeHandle, TypeQueryInfo>();
        public static readonly ConcurrentDictionary<string, string> UpdateFieldsQueryCache = new ConcurrentDictionary<string, string>();

        public static void FlushCache()
        {
            QueryInfos.Clear();
            UpdateFieldsQueryCache.Clear();
        }

        public static TypeQueryInfo GetQueryInfo(IDbConnection connection, TypeInfo typeInfo)
        {
            if (QueryInfos.TryGetValue(typeInfo.TypeHandle, out var typeQueryInfo))
            {
                return typeQueryInfo;
            }

            typeQueryInfo = CreateQueryInfo(connection, typeInfo);

            return typeQueryInfo;
        }

        public static IDictionary<string, ISqlDbHelper> SupportedHelpers { get; } = new Dictionary<string, ISqlDbHelper>()
        {
            { DefaultConnection, new SqlServerDbHelper() },
            { "mysqlconnection", new MySqlDbHelper() }
        };

        public static string GetSurrogateKeyReturnQuery(IDbConnection connection)
        {
            var sqlHelper = GetSqlHelper(connection);

            return sqlHelper.GetSurrogateKeyReturnQuery();
        }

        public static string GetInsertManyQuery(IDbConnection connection, TypeInfo typeInfo, TypeQueryInfo queryInfo, 
            int entitiesCount)
        {
            var surrogateKeySql = typeInfo.KeyType == KeyType.Surrogate ? $"{GetSurrogateKeyReturnQuery(connection)};" : string.Empty;

            var sb = new StringBuilder();
            var sbSql = new StringBuilder();
            var sqlHelper = GetSqlHelper(connection);

            for (int i = 0; i < entitiesCount; i++)
            {
                GenerateColumnSequence(sb, typeInfo.InsertableProperties, sqlHelper, true, $"_{i}");

                var insertParams = sb.ToString();

                sbSql.AppendLine($"{queryInfo.InsertNoValuesQuery.Replace("PARAMS", insertParams)};{surrogateKeySql}");

                sb.Clear();
            }

            var finalSql = sbSql.ToString();

            return finalSql;
        }

        public static string GetInsertQuery(IDbConnection connection, TypeInfo typeInfo, TypeQueryInfo queryInfo)
        {
            var surrogateKeySql = typeInfo.KeyType == KeyType.Surrogate ? $"{GetSurrogateKeyReturnQuery(connection)};" : string.Empty;
            return $"{queryInfo.InsertQuery};{surrogateKeySql};";
        }

        public static string GetUpdateFieldsQuery(IDbConnection connection, TypeInfo typeInfo, TypeQueryInfo queryInfo, 
            IEnumerable<string> fields, bool exclude = false)
        {
            var key = $"{string.Join(null, fields)}{exclude}";

            if (UpdateFieldsQueryCache.TryGetValue(key, out var sql))
            {
                return sql;
            }

            var sb = new StringBuilder();
            var sqlHelper = GetSqlHelper(connection);
            var props = exclude ?
                typeInfo.WritableProperties.Where(p => !fields.Contains(p.Name)) :
                typeInfo.WritableProperties.Where(p => fields.Contains(p.Name));

            GenerateColumnEqualsParamSequence(props, sqlHelper, sb, ", ");

            var sqlFields = sb.ToString();
            sql = queryInfo.UpdateFieldsQuery.Replace("FIELDS", sqlFields);

            UpdateFieldsQueryCache.TryAdd(key, sql);

            return sql;
        }

        public static string GetParamName(PropertyInfo property, string paramSufix = "")
        {
            return $"@{property.Name}{paramSufix}";
        }

        private static TypeQueryInfo CreateQueryInfo(IDbConnection connection, TypeInfo typeInfo)
        {
            TypeQueryInfo typeQueryInfo = new TypeQueryInfo();
            ISqlDbHelper sqlHelper = GetSqlHelper(connection);

            var sb = new StringBuilder();

            GenerateColumnEqualsParamSequence(typeInfo.PrimaryKeyProperties, sqlHelper, sb, " and ");

            var keyEqualsParams = sb.ToString(); // Id1 = @Id1 and Id2 = @Id2

            sb.Clear();

            GenerateColumnEqualsParamSequence(typeInfo.WritableProperties, sqlHelper, sb, ", ");

            var updateColumnsEqualsParams = sb.ToString(); // Attr1 = @Attr1, Attr2 = @Attr2

            sb.Clear();

            GenerateColumnSequence(sb, typeInfo.ReadableProperties, sqlHelper);

            var readableColumns = sb.ToString(); // Attr1, Attr2

            sb.Clear();

            GenerateColumnSequence(sb, typeInfo.InsertableProperties, sqlHelper); // Id1, Id2, Attr1, Attr2

            var insertColumns = sb.ToString();

            sb.Clear();

            GenerateColumnSequence(sb, typeInfo.InsertableProperties, sqlHelper, true); // @Id1, @Id2, @Attr1, @Attr2

            var insertParams = sb.ToString();

            var tableName = sqlHelper.FormatDbEntityName(typeInfo.TableName);

            typeQueryInfo.SelectQuery = string.Format(SelectTemplate, readableColumns, tableName, keyEqualsParams);
            typeQueryInfo.SelectAllQuery = string.Format(SelectAllTemplate, readableColumns, tableName);
            typeQueryInfo.InsertQuery = string.Format(InsertTemplate, tableName, insertColumns, insertParams);
            typeQueryInfo.InsertNoValuesQuery = string.Format(InsertNoValuesTemplate, tableName, insertColumns);
            typeQueryInfo.UpdateQuery = string.Format(UpdateTemplate, tableName, updateColumnsEqualsParams, keyEqualsParams);
            typeQueryInfo.UpdateFieldsQuery = string.Format(UpdateFieldsTemplate, tableName, keyEqualsParams);
            typeQueryInfo.DeleteQuery = string.Format(DeleteTemplate, tableName, keyEqualsParams);
            typeQueryInfo.DeleteAllQuery = string.Format(DeleteAllTemplate, tableName);

            QueryInfos.TryAdd(typeInfo.TypeHandle, typeQueryInfo);
            return typeQueryInfo;
        }

        private static ISqlDbHelper GetSqlHelper(IDbConnection connection)
        {
            var connectionName = connection.GetType().Name.ToLower();

            var sqlHelper = SupportedHelpers.TryGetValue(connectionName, out var helper)
                ? helper
                : SupportedHelpers[DefaultConnection];

            return sqlHelper;
        }

        private static string GetColumnEqualsToParam(string columnName, ISqlDbHelper sqlHelper)
        {
            return string.Format(ColumnEqualsToParamTemplate, sqlHelper.FormatDbEntityName(columnName), columnName);
        }

        private static void GenerateColumnSequence(StringBuilder sb, IEnumerable<PropertyInfo> properties, ISqlDbHelper sqlHelper, 
            bool forParams = false, string paramSufix = "")
        {
            for (var i = 0; i < properties.Count(); i++)
            {
                var property = properties.ElementAt(i);
                sb.Append(forParams ? GetParamName(property, paramSufix) : sqlHelper.FormatDbEntityName(property.Name));
                if (i < properties.Count() - 1)
                    sb.AppendFormat(", ");
            }
        }

        private static void GenerateColumnEqualsParamSequence(IEnumerable<PropertyInfo> properties, ISqlDbHelper sqlHelper, 
            StringBuilder sb, string separator)
        {
            for (var i = 0; i < properties.Count(); i++)
            {
                var property = properties.ElementAt(i);
                sb.Append(GetColumnEqualsToParam(property.Name, sqlHelper));
                if (i < properties.Count() - 1)
                    sb.AppendFormat(separator);
            }
        }
    }
}