using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Apex.Query
{
    public interface ISqlDbHelper
    {
        /// <summary>
        /// Gets the query that fetches the surrogate key from the last insert operation.
        /// </summary>
        /// <returns>Surrogate key return query.</returns>
        string GetSurrogateKeyReturnQuery();

        /// <summary>
        /// Formats the column, table or database name to be used in sql queries.
        /// </summary>
        /// <param name="entityName">The name of the column, table or database to be formatted.</param>
        /// <returns>The formatted column, table or database name.</returns>
        string FormatDbEntityName(string entityName);

        /// <summary>
        /// Get the query that returns a single value of 0 or 1 if the given test is passed.
        /// </summary>
        /// <param name="existsQueryTest">The selecte query to be tested by the exists function.</param>
        /// <returns>A query that checks the existence of a record.</returns>
        string GetExistsQuery(string existsQueryTest);
    }
}
