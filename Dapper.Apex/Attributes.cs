using System;

namespace Dapper.Apex
{
    /// <summary>
    /// Defines the name of the database table that refers to a class of objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Creates a table attribute with a given table name
        /// </summary>
        /// <param name="tableName">The name of the table in the database</param>
        public TableAttribute(string tableName)
        {
            Name = tableName;
        }

        /// <summary>
        /// The name of the table in the database
        /// </summary>
        public string Name { get; private set; }
    }

    /// <summary>
    /// Defines that the property of a class refers to a surrogate primary key in the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
    }

    /// <summary>
    /// Defines that the property of a class refers to a natural key in the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExplicitKeyAttribute : Attribute
    {
    }

    /// <summary>
    /// Defines that the property of a class will not only be used for data retrieval in the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReadOnlyAttribute : Attribute
    {
    }

    /// <summary>
    /// Defines that the property of a class will be ignored for all database operations
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputedAttribute : Attribute
    {
    }
}
