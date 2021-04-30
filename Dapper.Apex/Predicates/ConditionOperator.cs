using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Apex
{
    public class ConditionOperator : IEquatable<ConditionOperator>

    {
        public static ConditionOperator And = new ConditionOperator(" AND ");
        public static ConditionOperator Or = new ConditionOperator(" OR ");

        private ConditionOperator(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }

        public bool Equals(ConditionOperator other)
        {
            return string.Equals(this.Value, other.Value);
        }
    }
}
