using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Apex
{
    public class CompareOperator : IEquatable<CompareOperator>

    {
        public static CompareOperator Equal = new CompareOperator("=");
        public static CompareOperator NotEqual = new CompareOperator("<>");
        public static CompareOperator GreaterThan = new CompareOperator(">");
        public static CompareOperator GreatOrEqual = new CompareOperator(">=");
        public static CompareOperator LowerThan = new CompareOperator("<");
        public static CompareOperator LowerOrEqual = new CompareOperator("<=");
        public static CompareOperator Like = new CompareOperator("LIKE");

        private CompareOperator(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }

        public bool Equals(CompareOperator other)
        {
            return string.Equals(this.Value, other.Value);
        }
    }
}
