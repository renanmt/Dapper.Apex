using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Apex
{
    /// <summary>
    /// Exception raised from Dapper.Apex
    /// </summary>
    public class DapperApexException : Exception
    {
        /// <summary>
        /// Default constructor with exception message
        /// </summary>
        /// <param name="message">Exception message</param>
        internal DapperApexException(string message) : base(message)
        {
        }

        /// <summary>
        /// Secondary constructor with exception message and inner exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        internal DapperApexException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
