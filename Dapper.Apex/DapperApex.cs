using Dapper;
using Dapper.Apex.Query;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Dapper.Apex
{
    /// <summary>
    /// Extension methods for Dapper
    /// </summary>
    public static partial class DapperApex
    {
        public static void Initialize(IEnumerable<Type> modelTypes, IDbConnection dbConnection = null)
        {
            var createQueries = dbConnection != null;

            foreach (var type in modelTypes)
            {
                var typeInfo = TypeHelper.GetTypeInfo(type);

                if (createQueries)
                    QueryHelper.GetQueryInfo(dbConnection, typeInfo);
            }
        }

        public static void Initialize(Assembly assembly, string modelsNamespace, IDbConnection dbConnection = null)
        {
            var types = GetNamespaceClassTypes(assembly, modelsNamespace);

            Initialize(types, dbConnection);
        }

        public static void Initialize(Assembly assembly, string modelsNamespace, Type baseType, IDbConnection dbConnection = null)
        {
            var types = GetNamespaceClassTypes(assembly, modelsNamespace)
                .Where(type => type.IsAssignableTo(baseType));

            Initialize(types, dbConnection);
        }

        private static IEnumerable<Type> GetNamespaceClassTypes(Assembly assembly, string modelsNamespace)
        {
            return from type in assembly.GetTypes()
                   where !type.IsAbstract && type.IsClass && type.Namespace == modelsNamespace
                   select type;
        }

        private static DynamicParameters GenerateGetParams(Type type, dynamic keyObject, IEnumerable<PropertyInfo> keyProperties)
        {
            var dynParms = new DynamicParameters();
            Type keyObjectType = keyObject?.GetType();

            if (keyObject is ITuple)
            {
                var keyTuple = keyObject as ITuple;

                if (keyTuple.Length != keyProperties.Count())
                    throw new DapperApexException($"The tuple passed as key does not match the keys in {type.Name}.");

                for (var i = 0; i < keyProperties.Count(); i++)
                {
                    var property = keyProperties.ElementAt(i);

                    dynParms.Add($"@{property.Name}", keyTuple[i]);
                }
            }
            else if (keyProperties.Count() == 1 && (keyObjectType == null || keyObjectType.IsValueType || keyObjectType.Equals(typeof(string))))
            {
                dynParms.Add($"@{keyProperties.First().Name}", keyObject);
            }
            else
            {
                if (keyObjectType == null || keyObjectType.IsValueType)
                    throw new DapperApexException($"They key object passed cannot be null or value type for composite key in {type.Name}.");

                var isParamSameType = keyObjectType.Equals(type);
                var keyTypeProperties = isParamSameType ? null : keyObjectType.GetProperties();

                for (var i = 0; i < keyProperties.Count(); i++)
                {
                    var property = keyProperties.ElementAt(i);

                    var paramProperty = isParamSameType ? property : // if key object passed is the same type as expected result, use property
                        keyTypeProperties.FirstOrDefault(p => p.Name.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase));

                    if (paramProperty == null)
                        throw new DapperApexException($"The key object passed does not contain {property.Name} property.");

                    dynParms.Add($"@{property.Name}", paramProperty.GetValue(keyObject, null));
                }
            }

            return dynParms;
        }
    }
}
