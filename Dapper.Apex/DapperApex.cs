using Dapper.Apex.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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

        /// <summary>
        /// Creates and return a list of parameters to be used in Dapper based on a given key object.
        /// </summary>
        /// <param name="type">The type of your target Entity.</param>
        /// <param name="keyObject">The tuple, value, collection, dictionary, expando object or object that represents the target entity key.</param>
        /// <param name="keyProperties">The list of key properties of the target entity.</param>
        /// <returns>A DynamicParameters object representing all the parameters.</returns>
        public static DynamicParameters GetParameters(Type type, object keyObject, IEnumerable<PropertyInfo> keyProperties)
        {
            Type keyObjectType = keyObject?.GetType();

            if (keyObject is ITuple)
            {
                return GetParametersFromTuple(type, keyObject as ITuple, keyProperties);
            }
            else if (keyObjectType == null || keyObjectType.IsValueType || keyObjectType.Equals(typeof(string)))
            {
                return GetParametersFromValue(type, keyObject, keyProperties);
            }
            else if (keyObject is ExpandoObject)
            {
                return GetParametersFromExpandoObject(type, keyObject as ExpandoObject, keyProperties);
            }
            else if (keyObject is IEnumerable)
            {
                return keyObject is IDictionary ?
                    GetParametersFromDictionary(type, keyObject as IDictionary, keyProperties) :
                    GetParametersFromCollection(type, keyObject as IEnumerable, keyProperties);
            }
            else
            {
                return GetParemetersFromObject(type, keyObject, keyProperties, keyObjectType);
            }
        }

        private static IEnumerable<Type> GetNamespaceClassTypes(Assembly assembly, string modelsNamespace)
        {
            return from type in assembly.GetTypes()
                   where !type.IsAbstract && type.IsClass && type.Namespace == modelsNamespace
                   select type;
        }

        private static DynamicParameters GetParametersFromTuple(Type type, ITuple key, IEnumerable<PropertyInfo> keyProperties)
        {
            var propCount = keyProperties.Count();

            if (key.Length != propCount)
                throw new DapperApexException($"The tuple passed as key does not match the number of keys ({propCount}) in {type.Name}.");

            DynamicParameters dynParams = new DynamicParameters();

            for (var i = 0; i < propCount; i++)
            {
                var property = keyProperties.ElementAt(i);

                dynParams.Add(property.Name, key[i]);
            }

            return dynParams;
        }

        private static DynamicParameters GetParametersFromValue(Type type, dynamic key, IEnumerable<PropertyInfo> keyProperties)
        {
            var propCount = keyProperties.Count();

            if (propCount > 1)
                throw new DapperApexException($"The key passed does not match the number of keys ({propCount}) in {type.Name}.");

            DynamicParameters dynParams = new DynamicParameters();
            dynParams.Add($"@{keyProperties.First().Name}", key);
            return dynParams;
        }

        private static DynamicParameters GetParametersFromCollection(Type type, IEnumerable key, IEnumerable<PropertyInfo> keyProperties)
        {
            DynamicParameters dynParams = new DynamicParameters();
            IEnumerator keyEntries = key.GetEnumerator();

            var i = 0;
            var propCount = keyProperties.Count();

            while (keyEntries.MoveNext() && i < propCount)
            {
                var property = keyProperties.ElementAt(i);
                dynParams.Add(property.Name, keyEntries.Current);
                i++;
            }

            if (i < propCount)
                throw new DapperApexException($"Key collection contains less values ({i}) than primary keys ({propCount}) in the target type ({type.Name}).");

            return dynParams;
        }

        private static DynamicParameters GetParametersFromDictionary(Type type, IDictionary key, IEnumerable<PropertyInfo> keyProperties)
        {
            DynamicParameters dynParams = new DynamicParameters();
            var propCount = keyProperties.Count();

            if (key.Count < propCount)
                throw new DapperApexException($"Key dictionary contains less values ({key.Count}) than primary keys ({propCount}) in the target type ({type.Name}).");

            foreach (var property in keyProperties)
            {
                if (!key.Contains(property.Name))
                    throw new DapperApexException($"Key dictionary does not contain key name {property.Name} from target type ({type.Name}).");

                dynParams.Add(property.Name, key[property.Name]);
            }

            return dynParams;
        }

        private static DynamicParameters GetParametersFromExpandoObject(Type type, ExpandoObject key, IEnumerable<PropertyInfo> keyProperties)
        {
            DynamicParameters dynParams = new DynamicParameters();
            var propCount = keyProperties.Count();

            var dictionaryKey = (IDictionary<string, object>)key;

            if (dictionaryKey.Count < propCount)
                throw new DapperApexException($"ExpandoObject key contains less values ({dictionaryKey.Count}) than primary keys ({propCount}) in the target type ({type.Name}).");

            foreach (var property in keyProperties)
            {
                if (!dictionaryKey.ContainsKey(property.Name))
                    throw new DapperApexException($"ExpandoObject key does not contain key name {property.Name} from target type ({type.Name}).");

                dynParams.Add(property.Name, dictionaryKey[property.Name]);
            }

            return dynParams;
        }

        private static DynamicParameters GetParemetersFromObject(Type type, object keyObject, IEnumerable<PropertyInfo> keyProperties, Type keyObjectType)
        {
            if (keyObjectType == null || keyObjectType.IsValueType)
                throw new DapperApexException($"They key object passed cannot be null or value type for composite key in {type.Name}.");

            DynamicParameters dynParams = new DynamicParameters();

            var isParamSameType = keyObjectType.Equals(type);
            var keyTypeProperties = isParamSameType ? null : keyObjectType.GetProperties();

            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var property = keyProperties.ElementAt(i);

                var paramProperty = isParamSameType ? property : // if key object passed is the same type as expected result, use property
                    keyTypeProperties.FirstOrDefault(p => p.Name.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase));

                if (paramProperty == null)
                    throw new DapperApexException($"The key object passed does not contain the key property {property.Name} as in {type.Name}.");

                dynParams.Add($"@{property.Name}", paramProperty.GetValue(keyObject, null));
            }

            return dynParams;
        }
    }
}
