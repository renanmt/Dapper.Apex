using Dapper.Apex.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Apex
{
    public interface IWhere
    {
        string Sql { get; }
        DynamicParameters Params { get; }
    }

    public interface IWhere<T> : IWhere
    {
        IWhere<T> Add(Expression<Func<T, object>> expression, CompareOperator clauseOperator, object value);
        IWhere<T> Add(Expression<Func<T, object>> expression, CompareOperator clauseOperator, object value, ConditionOperator conditionOperator);
        IWhere<T> And(Expression<Func<T, object>> expression, CompareOperator clauseOperator, object value);
        IWhere<T> Or(Expression<Func<T, object>> expression, CompareOperator clauseOperator, object value);

        IWhere<T> AddGroup();
        IWhere<T> AddGroup(ConditionOperator conditionOperator);
        IWhere<T> AndGroup();
        IWhere<T> OrGroup();
        IWhere<T> EndGroup();
        IWhere Build(IDbConnection connection);
    }

    public static class Where<T> where T: class
    {
        public static IWhere<T> With(Expression<Func<T, object>> expression, CompareOperator clauseOperator, object value)
        {
            var where = new WhereClause<T>();
            where.Parts.Add(new Condition(expression.GetPropertyName(), clauseOperator, value));
            return where;
        }

        public static IWhere<T> WithGroup()
        {
            var where = new WhereClause<T>();
            where.Parts.Add(GroupBegin.Default);
            return where;
        }
    }

    public class WhereClause<T> : IWhere<T> where T : class
    { 
        internal WhereClause()
        {
        }

        private string _sql;
        private DynamicParameters _params;
        private int _unclosedGroups = 0;

        public IList<IWherePart> Parts { get; } = new List<IWherePart>();

        public string Sql => _sql;
        public DynamicParameters Params => _params;

        public IWhere<T> Add(Expression<Func<T, object>> expression, CompareOperator clauseOperator, object value)
        {
            this.And(expression, clauseOperator, value);
            return this;
        }

        public IWhere<T> Add(Expression<Func<T, object>> expression, CompareOperator clauseOperator, object value, ConditionOperator conditionOperator)
        {
            var conditionOp = Parts.Last().Name == "(" ? null : conditionOperator;
            Parts.Add(new Condition(expression.GetPropertyName(), clauseOperator, value, conditionOp));
            return this;
        }

        public IWhere<T> And(Expression<Func<T, object>> expression, CompareOperator clauseOperator, object value)
        {
            this.Add(expression, clauseOperator, value, ConditionOperator.And);
            return this;
        }

        public IWhere<T> Or(Expression<Func<T, object>> expression, CompareOperator clauseOperator, object value)
        {
            this.Add(expression, clauseOperator, value, ConditionOperator.Or);
            return this;
        }

        public IWhere<T> AddGroup()
        {
            this.AndGroup();
            return this;
        }

        public IWhere<T> AddGroup(ConditionOperator conditionOperator)
        {
            GroupBegin gb = GroupBegin.Default;
            if (conditionOperator != null)
            {
                gb = conditionOperator.Equals(ConditionOperator.And) ? GroupBegin.And : GroupBegin.Or;
            }

            var groupBegin = Parts.Last().Name == "(" ? GroupBegin.Default : gb;
            this.Parts.Add(groupBegin);
            _unclosedGroups++;
            return this;
        }

        public IWhere<T> AndGroup()
        {
            this.AddGroup(ConditionOperator.And);
            return this;
        }

        public IWhere<T> OrGroup()
        {
            this.AddGroup(ConditionOperator.Or);
            return this;
        }

        public IWhere<T> EndGroup()
        {
            if (_unclosedGroups == 0)
                throw new DapperApexException($"Invalid closing group at step {Parts.Count + 1}.");

            this.Parts.Add(GroupEnd.Default);
            return this;
        }

        public IWhere Build(IDbConnection connection)
        {
            var sqlHelper = QueryHelper.GetSqlHelper(connection);

            var sb = new StringBuilder();

            var paramNumber = 0;

            _params = new DynamicParameters();

            foreach (var part in Parts)
            {
                if (part is GroupBegin)
                {
                    var groupBegin = part as GroupBegin;
                    sb.Append($"{groupBegin.ConditionOperator}{part.Name}");
                }
                else if (part is GroupEnd)
                    sb.Append(part.Name);
                else if (part is Condition)
                {
                    var condition = part as Condition;
                    var paramName = QueryHelper.GetParamName(part.Name, $"_{paramNumber}");

                    sb.Append($"{condition.ConditionOperator}{sqlHelper.FormatDbEntityName(condition.Name)} {condition.ClauseOperator.ToString()} {paramName}");
                    _params.Add(paramName, condition.Value);

                    paramNumber++;
                }
            }

            _sql = sb.ToString();

            return this;
        }
    }

    public static class ExpressionExtensions
    {
        public static string GetPropertyName(this LambdaExpression expression)
        {
            Expression exp = expression;

            while (true)
            {
                switch (exp.NodeType)
                {
                    case ExpressionType.Lambda:
                        exp = (exp as LambdaExpression).Body;
                        break;
                    case ExpressionType.Convert:
                        exp = (exp as UnaryExpression).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        return (exp as MemberExpression).Member.Name;
                    default:
                        throw new DapperApexException("Unable to process expression. Property name not available.");
                }
            }
        }
    }
}
