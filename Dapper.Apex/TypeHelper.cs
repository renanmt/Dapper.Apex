using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dapper.Apex
{
    /// <summary>
    /// Defines the an entity type of key.
    /// </summary>
    public enum KeyType
    {
        Natural,
        Surrogate
    }

    /// <summary>
    /// Collection of property information of a type.
    /// </summary>
    public class TypeInfo
    {
        public RuntimeTypeHandle TypeHandle { get; set; }
        public string TableName { get; set; }
        public KeyType KeyType { get; set; }
        public IEnumerable<PropertyInfo> PrimaryKeyProperties { get; set; }
        public IEnumerable<PropertyInfo> WritableProperties { get; set; }
        public IEnumerable<PropertyInfo> ReadableProperties { get; set; }
        public IEnumerable<PropertyInfo> ComputedProperties { get; set; }
        public IEnumerable<PropertyInfo> InsertableProperties { get; set; }
    }

    /// <summary>
    /// A class that provides all property information of types.
    /// </summary>
    public static class TypeHelper
    {
        public static readonly ConcurrentDictionary<RuntimeTypeHandle, TypeInfo> TypeInfos = new ConcurrentDictionary<RuntimeTypeHandle, TypeInfo>();
        public static readonly ConcurrentDictionary<RuntimeTypeHandle, Type> ElementTypes = new ConcurrentDictionary<RuntimeTypeHandle, Type>();

        /// <summary>
        /// Flushes all type caches.
        /// </summary>
        public static void FlushCache()
        {
            TypeInfos.Clear();
            ElementTypes.Clear();
        }

        /// <summary>
        /// Gets property information of a type.
        /// </summary>
        /// <remarks>If infomartion is not yet cached, it will process and cache it for further usage</remarks>
        /// <param name="type">The type to be parsed</param>
        /// <returns>Full information about type properties</returns>
        public static TypeInfo GetTypeInfo(Type type)
        {
            if (TypeInfos.TryGetValue(type.TypeHandle, out var typeInfo))
            {
                return typeInfo;
            }

            return ProcessType(type);
        }

        private static TypeInfo ProcessType(Type type)
        {
            var tableName = type.GetCustomAttribute<TableAttribute>(false)?.Name ?? type.Name;

            var allTypeProperties = type.GetProperties();

            var surrogateKeys = allTypeProperties
                .Where(prop =>
                    prop.GetCustomAttribute<KeyAttribute>(true) != null ||
                    (prop.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase) && prop.GetCustomAttribute<ExplicitKeyAttribute>(true) == null)
                );

            var naturalKeys = allTypeProperties.Where(prop => prop.GetCustomAttribute<ExplicitKeyAttribute>(true) != null);

            var primaryKeyProperties = new List<PropertyInfo>();
            primaryKeyProperties.AddRange(surrogateKeys);
            primaryKeyProperties.AddRange(naturalKeys);

            if (!primaryKeyProperties.Any())
                throw new DapperApexException($"No keys found for {type.Name}. Try using [Key] or [ExplicitKey] attributes.");

            var keyType = KeyType.Natural;

            if (naturalKeys.Any() && surrogateKeys.Any())
            {
                throw new DapperApexException($"Type {type.Name} has mixed [Key] and [ExplicitKey] attributes.");
            }
            else if (surrogateKeys.Any())
            {
                if (surrogateKeys.Count() > 1)
                    throw new DapperApexException($"Type {type.Name} has multiple [Key] attributes.");

                keyType = KeyType.Surrogate;
            }

            var computedProperties = allTypeProperties
                .Where(prop =>
                    prop.GetCustomAttributes(true).Any(attr => attr is ComputedAttribute)
                );

            var readableProperties = allTypeProperties.Except(computedProperties);

            var writableProperties = readableProperties
                .Except(primaryKeyProperties)
                .Where(prop =>
                    !prop.GetCustomAttributes(true).Any(attr => attr is ReadOnlyAttribute)
                );

            var insertableProps = writableProperties.ToList();

            if (keyType == KeyType.Natural)
                insertableProps.AddRange(primaryKeyProperties);

            var typeInfo = new TypeInfo()
            {
                TypeHandle = type.TypeHandle,
                TableName = tableName,
                KeyType = keyType,
                PrimaryKeyProperties = primaryKeyProperties,
                ReadableProperties = readableProperties,
                WritableProperties = writableProperties,
                ComputedProperties = computedProperties,
                InsertableProperties = insertableProps
            };

            TypeInfos.TryAdd(type.TypeHandle, typeInfo);

            return typeInfo;
        }
    }
}
