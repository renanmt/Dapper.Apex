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
    /// Apex extension methods for Dapper.
    /// </summary>
    public static partial class DapperApex
    {
        /// <summary>
        /// Initialize the type and query caches for a list of entity types.
        /// </summary>
        /// <remarks>
        /// Initializing your types before usage frees you from the overhead of processing types during the first operations and also provides you
        /// quick validation of types at runtime.
        /// </remarks>
        /// <param name="entityTypes">The list of entity types.</param>
        /// <param name="connection">A database connection object to represent the origin of the given entity types.</param>
        public static void Initialize(IEnumerable<Type> entityTypes, IDbConnection connection = null)
        {
            var createQueries = connection != null;

            foreach (var type in entityTypes)
            {
                var typeInfo = TypeHelper.GetTypeInfo(type);

                if (createQueries)
                    QueryHelper.GetQueryInfo(connection, typeInfo);
            }
        }

        /// <summary>
        /// Initialize the type and query caches for a given assembly and namespace.
        /// </summary>
        /// <remarks>
        /// Initializing your types before usage frees you from the overhead of processing types during the first operations and also provides you
        /// quick validation of types at runtime.
        /// </remarks>
        /// <param name="assembly">The assembly containing the entity types.</param>
        /// <param name="entitiesNamespace">The namespace containing the entity types.</param>
        /// <param name="connection">A database connection object to represent the origin of the given entity types.</param>
        public static void Initialize(Assembly assembly, string entitiesNamespace, IDbConnection connection = null)
        {
            var types = GetNamespaceClassTypes(assembly, entitiesNamespace);

            Initialize(types, connection);
        }

        /// <summary>
        /// Initialize the type and query caches for a given assembly and namespace and a base type.
        /// </summary>
        /// <remarks>
        /// Initializing your types before usage frees you from the overhead of processing types of during first operations and also provides you
        /// quick validation of types at runtime.
        /// </remarks>
        /// <param name="assembly">The assembly containing the entity types.</param>
        /// <param name="entitiesNamespace">The namespace containing the entity types.</param>
        /// <param name="baseType">The common base type for entity types.</param>
        /// <param name="connection">A database connection object to represent the origin of the given entity types.</param>
        public static void Initialize(Assembly assembly, string entitiesNamespace, Type baseType, IDbConnection connection = null)
        {
            var types = GetNamespaceClassTypes(assembly, entitiesNamespace)
                .Where(type => baseType.IsAssignableFrom(type));

            Initialize(types, connection);
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
