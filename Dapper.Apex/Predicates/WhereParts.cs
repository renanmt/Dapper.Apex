using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Apex
{
    public sealed class GroupBegin : IWherePart
    {
        public static GroupBegin Default = new GroupBegin();
        public static GroupBegin And = new GroupBegin(ConditionOperator.And);
        public static GroupBegin Or = new GroupBegin(ConditionOperator.Or);


        public string Name => "(";
        public ConditionOperator ConditionOperator { get; private set; }


        private GroupBegin() {}
        private GroupBegin(ConditionOperator conditionOperator) { ConditionOperator = conditionOperator; }
    }

    public sealed class GroupEnd : IWherePart
    {
        public static GroupEnd Default = new GroupEnd();
        public string Name => ")";

        private GroupEnd() { }
    }

    public sealed class Condition : IWherePart
    {
        public Condition(string name, CompareOperator clauseOperator, object value)
        {
            Name = name;
            ClauseOperator = clauseOperator;
            Value = value;
        }

        public Condition(string name, CompareOperator clauseOperator, object value, ConditionOperator conditionOperator)
        {
            Name = name;
            ClauseOperator = clauseOperator;
            Value = value;
            ConditionOperator = conditionOperator;
        }

        public ConditionOperator ConditionOperator { get; private set; }
        public string Name { get; private set; }
        public CompareOperator ClauseOperator { get; private set; }
        public object Value { get; private set; }
    }
}
